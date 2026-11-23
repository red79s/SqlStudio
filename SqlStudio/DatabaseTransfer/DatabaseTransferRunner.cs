using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CfgDataStore;
using CommandPrompt;

namespace SqlStudio.DatabaseTransfer
{
    public enum TransferStep { Preflight, Backup, Download, Upload, Restore, Relink, Script, Cleanup }

    public enum StepStatus { Pending, Running, Succeeded, Failed, Skipped, Warning }

    public enum TransferOutcome { Succeeded, Failed, Cancelled }

    /// <summary>One progress notification. Any combination of fields may be set.</summary>
    public class TransferEvent
    {
        public TransferStep Step { get; set; }
        public StepStatus? Status { get; set; }

        /// <summary>0-100 for the current step, or null if unchanged.</summary>
        public double? Percent { get; set; }

        /// <summary>Short status text for the current step (e.g. "1.2 GB of 3.4 GB, 45 MB/s").</summary>
        public string Detail { get; set; }

        /// <summary>Line to append to the run log.</summary>
        public string LogLine { get; set; }
    }

    public class TransferException : Exception
    {
        public TransferException(string message) : base(message) { }
        public TransferException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Runs one prod -> test transfer:
    /// backup on prod (SQL) -> scp to this machine -> scp to test -> restore on test (SQL) -> optional script -> cleanup.
    /// </summary>
    public class DatabaseTransferRunner
    {
        /// <summary>SQL Server message number for "N percent processed." emitted by BACKUP/RESTORE WITH STATS.</summary>
        private const int PercentProcessedMessage = 3211;

        private const int SqlServerExpressEdition = 4;

        private static readonly TimeSpan CleanupTimeout = TimeSpan.FromMinutes(2);

        private readonly TransferPair _pair;
        private readonly IReadOnlyList<Connection> _connections;
        private readonly string _localFolder;
        private readonly IProgress<TransferEvent> _progress;
        private readonly SshClient _ssh;

        private StreamWriter _logWriter;
        private readonly object _logLock = new object();
        private CancellationToken _token;
        private TransferStep _currentStep;

        // Resolved during preflight
        private Connection _source;
        private Connection _target;
        private string _sourceHost;
        private string _targetHost;
        private string _backupFileName;
        private string _prodBackupPath;
        private string _localBackupPath;
        private string _testBackupPath;
        private string _scriptText;
        private List<UserLink> _relinks = new List<UserLink>();

        // What has happened so far, drives cleanup
        private bool _prodBackupMayExist;
        private bool _downloadComplete;
        private bool _uploadStarted;
        private bool _uploadComplete;
        private bool _restoreSucceeded;

        public DatabaseTransferRunner(TransferPair pair, IReadOnlyList<Connection> connections, string localFolder,
            IProgress<TransferEvent> progress, SshClient ssh = null, bool testRun = false)
        {
            TestRun = testRun;
            _pair = pair?.Clone() ?? throw new ArgumentNullException(nameof(pair));
            _connections = connections ?? throw new ArgumentNullException(nameof(connections));
            _localFolder = string.IsNullOrWhiteSpace(localFolder) ? TransferSettingsStore.DefaultLocalFolder : localFolder.Trim();
            _progress = progress;
            _ssh = ssh ?? new SshClient();
        }

        public string LogFilePath { get; private set; }

        /// <summary>
        /// Test run: back up prod and copy the backup to the test server, then stop. The restore command is
        /// built from the real backup file and test server settings and printed, but not executed; the test
        /// database and the post-restore script are left untouched.
        /// </summary>
        public bool TestRun { get; }

        /// <summary>In a test run, the restore script that would have been executed (null until known).</summary>
        public string PlannedRestoreSql { get; private set; }

        public async Task<TransferOutcome> RunAsync(CancellationToken token)
        {
            _token = token;
            var total = Stopwatch.StartNew();
            var outcome = TransferOutcome.Failed;
            string failure = null;

            try
            {
                Directory.CreateDirectory(_localFolder);
                LogFilePath = Path.Combine(_localFolder,
                    $"{(TestRun ? "TestRun" : "Transfer")}_{SafeFileName(_pair.Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                _logWriter = new StreamWriter(LogFilePath, false, Encoding.UTF8) { AutoFlush = true };
            }
            catch (Exception ex)
            {
                Log($"WARNING: cannot write log file in '{_localFolder}': {ex.Message}");
            }

            if (TestRun)
                Log("TEST RUN: backup and copy only. The test database will NOT be changed; the restore command is printed instead.");
            Log($"Transfer '{_pair.Name}' started by {Environment.UserDomainName}\\{Environment.UserName} on {Environment.MachineName}");

            try
            {
                await RunStepAsync(TransferStep.Preflight, () => PreflightAsync(token));
                await RunStepAsync(TransferStep.Backup, () => BackupAsync(token));
                await RunStepAsync(TransferStep.Download, () => DownloadAsync(token));
                await RunStepAsync(TransferStep.Upload, () => UploadAsync(token));
                if (TestRun)
                {
                    await RunStepAsync(TransferStep.Restore, () => PlanRestoreAsync(token));
                    Report(new TransferEvent { Step = TransferStep.Restore, Status = StepStatus.Succeeded, Detail = "Test run: restore command printed in log, not executed" });
                    Report(new TransferEvent { Step = TransferStep.Relink, Status = StepStatus.Skipped,
                        Detail = _relinks.Count > 0 ? "Test run: included in printed command, not executed" : "No users configured" });
                    Report(new TransferEvent { Step = TransferStep.Script, Status = StepStatus.Skipped,
                        Detail = _scriptText != null ? "Test run: not executed" : "No script configured" });
                }
                else
                {
                    await RunStepAsync(TransferStep.Restore, () => RestoreAsync(token));
                    if (_relinks.Count > 0)
                        await RunStepAsync(TransferStep.Relink, () => RelinkUsersAsync(token));
                    else
                        Report(new TransferEvent { Step = TransferStep.Relink, Status = StepStatus.Skipped, Detail = "No users configured" });
                }

                if (TestRun)
                {
                    // Nothing more to do.
                }
                else if (_scriptText != null)
                    await RunStepAsync(TransferStep.Script, () => RunScriptAsync(token));
                else
                    Report(new TransferEvent { Step = TransferStep.Script, Status = StepStatus.Skipped, Detail = "No script configured" });

                outcome = TransferOutcome.Succeeded;
            }
            catch (OperationCanceledException)
            {
                outcome = TransferOutcome.Cancelled;
                failure = "Cancelled by user.";
            }
            catch (Exception) when (token.IsCancellationRequested)
            {
                // SqlClient reports a cancelled command as a SqlException.
                outcome = TransferOutcome.Cancelled;
                failure = "Cancelled by user.";
            }
            catch (Exception ex)
            {
                outcome = TransferOutcome.Failed;
                failure = ex is TransferException || ex is SqlException ? ex.Message : ex.ToString();
            }

            if (failure != null)
            {
                Log((outcome == TransferOutcome.Cancelled ? "CANCELLED: " : "FAILED: ") + failure);
                if (outcome == TransferOutcome.Cancelled && _currentStep == TransferStep.Restore)
                    Log($"Restore was interrupted. Test database '{_target?.db}' may be left in RESTORING state and must be restored again or dropped.");
            }

            await CleanupAsync();

            total.Stop();
            Log($"Transfer {outcome.ToString().ToUpperInvariant()} after {FormatElapsed(total.Elapsed)}.");
            if (LogFilePath != null)
                Log($"Log file: {LogFilePath}");

            lock (_logLock)
            {
                _logWriter?.Dispose();
                _logWriter = null;
            }
            return outcome;
        }

        private async Task RunStepAsync(TransferStep step, Func<Task> body)
        {
            _currentStep = step;
            var sw = Stopwatch.StartNew();
            Report(new TransferEvent { Step = step, Status = StepStatus.Running, Percent = 0, Detail = "" });
            Log($"--- {step} ---");
            try
            {
                await body();
            }
            catch (Exception) when (_token.IsCancellationRequested)
            {
                Report(new TransferEvent { Step = step, Status = StepStatus.Failed, Detail = "Cancelled" });
                throw;
            }
            catch (Exception ex)
            {
                Report(new TransferEvent { Step = step, Status = StepStatus.Failed, Detail = FirstLine(ex.Message) });
                throw;
            }
            Log($"{step} finished in {FormatElapsed(sw.Elapsed)}");
            Report(new TransferEvent { Step = step, Status = StepStatus.Succeeded, Percent = 100, Detail = FormatElapsed(sw.Elapsed) });
        }

        #region Steps

        private async Task PreflightAsync(CancellationToken token)
        {
            var errors = TransferGuard.Validate(_pair, _connections);
            if (errors.Count > 0)
                throw new TransferException("Transfer is not allowed:" + Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => "  - " + e)));

            _source = TransferGuard.ResolveSource(_pair, _connections);
            _target = TransferGuard.ResolveTarget(_pair, _connections);
            _sourceHost = TransferGuard.ResolveSshHost(_pair.SourceSsh, _source);
            _targetHost = TransferGuard.ResolveSshHost(_pair.TargetSsh, _target);

            Log($"Source (prod): {_source.server} / {_source.db}  [ssh {Describe(_pair.SourceSsh, _sourceHost)}]");
            Log($"Target (test): {_target.server} / {_target.db}  [ssh {Describe(_pair.TargetSsh, _targetHost)}]");

            if (!_ssh.ToolsAvailable)
                throw new TransferException("OpenSSH client not found (" + _ssh.ToolsDescription + "). Install the Windows 'OpenSSH Client' optional feature.");
            Log("Using " + _ssh.ToolsDescription);

            if (!string.IsNullOrWhiteSpace(_pair.PostRestoreScriptPath))
            {
                var path = _pair.PostRestoreScriptPath.Trim();
                if (!File.Exists(path))
                    throw new TransferException($"Post-restore script not found: {path}");
                _scriptText = File.ReadAllText(path);
                Log($"Post-restore script: {path} ({SqlBatchSplitter.Split(_scriptText).Count} batches)");
            }

            _backupFileName = $"{SafeFileName(_source.db)}_{DateTime.Now:yyyyMMdd_HHmmss}_SqlStudioTransfer.bak";
            _prodBackupPath = SshClient.CombineWindowsPath(_pair.SourceSsh.Folder, _backupFileName);
            _testBackupPath = SshClient.CombineWindowsPath(_pair.TargetSsh.Folder, _backupFileName);
            _localBackupPath = Path.Combine(_localFolder, _backupFileName);

            Detail(TransferStep.Preflight, "Checking SQL connection to prod", 10);
            using (var con = await OpenAsync(_source, "master", token))
            {
                var state = await ScalarAsync<string>(con, "SELECT state_desc FROM sys.databases WHERE name = @db", token, ("@db", _source.db));
                if (state == null)
                    throw new TransferException($"Database '{_source.db}' does not exist on {_source.server}.");
                if (state != "ONLINE")
                    throw new TransferException($"Database '{_source.db}' on {_source.server} is {state}, not ONLINE.");
                var login = await ScalarAsync<string>(con, "SELECT SUSER_SNAME()", token);
                var canBackup = await ScalarAsync<int?>(con, "SELECT HAS_PERMS_BY_NAME(@db, 'DATABASE', 'BACKUP DATABASE')", token, ("@db", _source.db));
                if (canBackup != 1)
                    throw new TransferException(
                        $"Login '{login}' on {_source.server} does not have BACKUP DATABASE permission on '{_source.db}'. " +
                        $"It needs to be sysadmin, or db_owner / db_backupoperator in '{_source.db}'.");
                Log($"Prod SQL connection OK as '{login}', database is ONLINE, BACKUP permission OK");
            }

            Detail(TransferStep.Preflight, "Checking SQL connection to test", 30);
            using (var con = await OpenAsync(_target, "master", token))
            {
                var login = await ScalarAsync<string>(con, "SELECT SUSER_SNAME()", token);
                var exists = await ScalarAsync<int?>(con, "SELECT DB_ID(@db)", token, ("@db", _target.db));

                // RESTORE FILELISTONLY (needed even in a test run) and RESTORE of a new database both require
                // CREATE DATABASE permission; replacing an existing database also needs ALTER on it.
                var missing = new List<string>();
                var canCreate = await ScalarAsync<int?>(con, "SELECT HAS_PERMS_BY_NAME(NULL, NULL, 'CREATE ANY DATABASE')", token);
                if (canCreate != 1)
                    missing.Add("CREATE ANY DATABASE (needed to read the backup file list and to restore)");
                if (exists.HasValue && !TestRun)
                {
                    // dbcreator/sysadmin may alter and restore any database, but that comes from role membership,
                    // which HAS_PERMS_BY_NAME does not report (especially when the login has no user in the database).
                    var canAlter = await ScalarAsync<int?>(con,
                        "SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin') = 1 OR IS_SRVROLEMEMBER('dbcreator') = 1 " +
                        "OR (SELECT owner_sid FROM sys.databases WHERE name = @db) = SUSER_SID() " +
                        "OR HAS_PERMS_BY_NAME(@db, 'DATABASE', 'ALTER') = 1 THEN 1 ELSE 0 END",
                        token, ("@db", _target.db));
                    if (canAlter != 1)
                        missing.Add($"ALTER on database '{_target.db}' (needed to disconnect users and replace it)");
                }
                _relinks = UserLink.Parse(_pair.RelinkUsers);
                foreach (var link in _relinks)
                {
                    var loginSid = await ScalarAsync<int?>(con, "SELECT SUSER_ID(@login)", token, ("@login", link.Login));
                    if (!loginSid.HasValue)
                        throw new TransferException($"Re-link users: login '{link.Login}' does not exist on {_target.server}. Create it there or fix the user list.");
                }
                if (_relinks.Count > 0)
                    Log($"Users to re-link after restore: {string.Join(", ", _relinks)}" + (_pair.RelinkUsersAsDbOwner ? " (as db_owner)" : ""));

                if (missing.Count > 0)
                    throw new TransferException(
                        $"Login '{login}' on {_target.server} is missing permissions:" + Environment.NewLine +
                        string.Join(Environment.NewLine, missing.Select(m => "  - " + m)) + Environment.NewLine +
                        $"Fix on {_target.server}:  ALTER SERVER ROLE dbcreator ADD MEMBER {QuoteName(login ?? "login")};");

                Log($"Test SQL connection OK as '{login}', restore permissions OK, database '{_target.db}' " +
                    (exists.HasValue ? "exists and will be replaced" : "does not exist and will be created") +
                    (TestRun ? " (test run: will not be touched)" : ""));
            }

            Detail(TransferStep.Preflight, $"Checking ssh to {_sourceHost}", 50);
            await CheckFolderAsync(_pair.SourceSsh, _sourceHost, "prod", token);

            Detail(TransferStep.Preflight, $"Checking ssh to {_targetHost}", 75);
            await CheckFolderAsync(_pair.TargetSsh, _targetHost, "test", token);
        }

        private async Task CheckFolderAsync(SshEndpoint ep, string host, string side, CancellationToken token)
        {
            var r = await _ssh.CheckRemoteFolderAsync(host, ep.User, ep.Port, ep.Folder, token);
            if (!r.Success)
                throw new TransferException($"ssh check of {side} server {host} folder '{ep.Folder}' failed ({r.Describe()}).");
            Log($"ssh to {side} server {host} OK, folder '{ep.Folder}' exists");
        }

        private async Task BackupAsync(CancellationToken token)
        {
            using var con = await OpenAsync(_source, "master", token);

            var edition = await ScalarAsync<int?>(con, "SELECT CAST(SERVERPROPERTY('EngineEdition') AS int)", token);
            var options = new List<string> { "COPY_ONLY" };
            if (edition == SqlServerExpressEdition)
                Log("Prod server is Express edition; backup compression is not available.");
            else
                options.Add("COMPRESSION");
            options.Add("INIT");
            options.Add("STATS = 5");

            var sql = $"BACKUP DATABASE {QuoteName(_source.db)} TO DISK = {QuoteString(_prodBackupPath)} WITH {string.Join(", ", options)}";
            Log(sql);

            _prodBackupMayExist = true;
            await ExecuteWithStatsAsync(con, sql, TransferStep.Backup, token);
            Log($"Backup written to {_sourceHost}:{_prodBackupPath}");
        }

        private async Task DownloadAsync(CancellationToken token)
        {
            var ep = _pair.SourceSsh;
            long total = await _ssh.GetRemoteFileSizeAsync(_sourceHost, ep.User, ep.Port, _prodBackupPath, token);
            if (total < 0)
                throw new TransferException($"Backup file {_prodBackupPath} not found on {_sourceHost} (does the SQL Server folder match the ssh folder?).");
            Log($"Backup size: {FormatBytes(total)}");

            var drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(_localFolder)));
            if (drive.IsReady && drive.AvailableFreeSpace < total)
                throw new TransferException($"Not enough free space in {_localFolder}: need {FormatBytes(total)}, have {FormatBytes(drive.AvailableFreeSpace)}.");

            Log($"scp {_sourceHost}:{_prodBackupPath} -> {_localBackupPath}");
            var sw = Stopwatch.StartNew();
            using var pollCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var poll = PollAsync(TransferStep.Download, total, sw, () =>
            {
                try
                {
                    var fi = new FileInfo(_localBackupPath);
                    return Task.FromResult(fi.Exists ? fi.Length : 0L);
                }
                catch (IOException)
                {
                    return Task.FromResult(-1L);
                }
            }, TimeSpan.FromMilliseconds(500), pollCts.Token);

            SshCommandResult r;
            try
            {
                r = await _ssh.DownloadAsync(_sourceHost, ep.User, ep.Port, _prodBackupPath, _localBackupPath, token);
            }
            finally
            {
                pollCts.Cancel();
                await IgnoreCancel(poll);
            }

            if (!r.Success)
                throw new TransferException($"scp download failed ({r.Describe()}).");

            long local = new FileInfo(_localBackupPath).Length;
            if (local != total)
                throw new TransferException($"Downloaded file size {local} does not match backup size {total}.");

            _downloadComplete = true;
            Log($"Downloaded {FormatBytes(total)} in {FormatElapsed(sw.Elapsed)} ({FormatRate(total, sw.Elapsed)})");
        }

        private async Task UploadAsync(CancellationToken token)
        {
            var ep = _pair.TargetSsh;
            long total = new FileInfo(_localBackupPath).Length;

            Log($"scp {_localBackupPath} -> {_targetHost}:{_testBackupPath}");
            var sw = Stopwatch.StartNew();
            using var pollCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            // Remote size is checked over ssh, which takes a second or two per check.
            var poll = PollAsync(TransferStep.Upload, total, sw,
                () => _ssh.GetRemoteFileSizeAsync(_targetHost, ep.User, ep.Port, _testBackupPath, pollCts.Token),
                TimeSpan.FromSeconds(3), pollCts.Token);

            _uploadStarted = true;
            SshCommandResult r;
            try
            {
                r = await _ssh.UploadAsync(_targetHost, ep.User, ep.Port, _localBackupPath, _testBackupPath, token);
            }
            finally
            {
                pollCts.Cancel();
                await IgnoreCancel(poll);
            }

            if (!r.Success)
                throw new TransferException($"scp upload failed ({r.Describe()}).");

            long remote = await _ssh.GetRemoteFileSizeAsync(_targetHost, ep.User, ep.Port, _testBackupPath, token);
            if (remote != total)
                throw new TransferException($"Uploaded file size on test server ({remote}) does not match local size ({total}).");

            _uploadComplete = true;
            Log($"Uploaded {FormatBytes(total)} in {FormatElapsed(sw.Elapsed)} ({FormatRate(total, sw.Elapsed)})");
        }

        private async Task RestoreAsync(CancellationToken token)
        {
            var db = _target.db;
            using var con = await OpenAsync(_target, "master", token);
            var sql = await BuildRestoreSqlAsync(con, token);
            Log(sql);

            try
            {
                await ExecuteWithStatsAsync(con, sql, TransferStep.Restore, token);
            }
            finally
            {
                // The restored database inherits prod's access mode, and a failed restore may have left it
                // single-user. Either way make sure it is usable again.
                await TrySetMultiUserAsync(db);
            }

            _restoreSucceeded = true;
            Log($"Database '{db}' restored on {_target.server}");
            await EnsureRestoringLoginCanOpenAsync(con, token);
        }

        /// <summary>
        /// After a restore, sys.databases lists the restoring login as owner, but dbo inside the database still has
        /// prod's owner SID, so the restoring login cannot open the database (needed for re-link and the script).
        /// Re-assigning the owner to the same login syncs dbo's SID.
        /// </summary>
        private async Task EnsureRestoringLoginCanOpenAsync(SqlConnection con, CancellationToken token)
        {
            var db = _target.db;
            var access = await ScalarAsync<int?>(con, "SELECT HAS_DBACCESS(@db)", token, ("@db", db));
            if (access == 1)
                return;

            var login = await ScalarAsync<string>(con, "SELECT SUSER_SNAME()", token);
            var sql = BuildFixOwnerSql(db, login);
            Log($"Login '{login}' cannot open '{db}' (dbo still has prod's owner SID). Fixing the owner:");
            Log(sql);
            try
            {
                using var cmd = new SqlCommand(sql, con) { CommandTimeout = 120 };
                await cmd.ExecuteNonQueryAsync(token);
                Log($"Database owner set to '{login}'; it now opens the database as dbo.");
            }
            catch (SqlException ex) when (!token.IsCancellationRequested)
            {
                Log($"WARNING: could not set the database owner: {ex.Message}");
            }
        }

        /// <summary>Makes <paramref name="login"/> the owner of <paramref name="db"/>, which maps it to dbo inside the database.</summary>
        public static string BuildFixOwnerSql(string db, string login) =>
            $"ALTER AUTHORIZATION ON DATABASE::{QuoteName(db)} TO {QuoteName(login)};";

        /// <summary>Test run: build the restore script from the uploaded backup and print it without running it.</summary>
        private async Task PlanRestoreAsync(CancellationToken token)
        {
            var db = _target.db;
            using var con = await OpenAsync(_target, "master", token);
            var restore = await BuildRestoreSqlAsync(con, token);

            var script = new StringBuilder();
            script.AppendLine($"-- Restore {_source.server}/{_source.db} onto {_target.server}/{db}");
            script.AppendLine($"-- Run on {_target.server} while the backup file still exists: {_testBackupPath}");
            script.AppendLine("USE master;");
            script.AppendLine("GO");
            script.AppendLine(restore);
            script.AppendLine("GO");
            script.AppendLine($"ALTER DATABASE {QuoteName(db)} SET MULTI_USER;");
            script.AppendLine("GO");
            var restoringLogin = await ScalarAsync<string>(con, "SELECT SUSER_SNAME()", token);
            script.AppendLine("-- Only if the restoring login cannot open the database afterwards (dbo still has prod's owner SID):");
            script.AppendLine($"IF HAS_DBACCESS({QuoteString(db)}) = 0");
            script.AppendLine("    " + BuildFixOwnerSql(db, restoringLogin));
            script.AppendLine("GO");
            if (_relinks.Count > 0)
            {
                script.AppendLine($"USE {QuoteName(db)};");
                script.AppendLine("GO");
                script.AppendLine(BuildRelinkSql(_relinks, _pair.RelinkUsersAsDbOwner));
                script.AppendLine("GO");
            }
            if (_scriptText != null)
                script.AppendLine($"-- Then run the post-restore script against [{db}]: {_pair.PostRestoreScriptPath.Trim()}");
            PlannedRestoreSql = script.ToString();

            const string bar = "==================================================================";
            Log("TEST RUN: the restore was NOT executed. Command that would have been run:");
            Log(bar);
            foreach (var line in PlannedRestoreSql.TrimEnd().Split('\n'))
                Log(line.TrimEnd('\r'));
            Log(bar);
        }

        /// <summary>
        /// Reads the backup's file list and the test server's default folders (read-only) and builds the
        /// ALTER DATABASE ... SINGLE_USER (if the database exists) + RESTORE ... WITH MOVE batch.
        /// </summary>
        private async Task<string> BuildRestoreSqlAsync(SqlConnection con, CancellationToken token)
        {
            var db = _target.db;
            Detail(TransferStep.Restore, "Reading backup file list", 0);
            var files = await ReadFileListAsync(con, token);

            var dataPath = await ScalarAsync<string>(con, "SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(4000))", token);
            var logPath = await ScalarAsync<string>(con, "SELECT CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS nvarchar(4000))", token);
            if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
            {
                // Pre-2012 servers: use the folders of the model database.
                var modelData = await ScalarAsync<string>(con, "SELECT TOP 1 physical_name FROM sys.master_files WHERE database_id = DB_ID('model') AND type = 0", token);
                var modelLog = await ScalarAsync<string>(con, "SELECT TOP 1 physical_name FROM sys.master_files WHERE database_id = DB_ID('model') AND type = 1", token);
                if (string.IsNullOrWhiteSpace(dataPath)) dataPath = WindowsDirectoryName(modelData);
                if (string.IsNullOrWhiteSpace(logPath)) logPath = WindowsDirectoryName(modelLog);
            }
            if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
                throw new TransferException("Could not determine the default data/log folders on the test server.");
            Log($"Test server default data folder: {dataPath}, log folder: {logPath}");

            var moves = BuildMoveClauses(files, db, dataPath, logPath);
            foreach (var m in moves)
                Log($"  MOVE {m.Logical} -> {m.Physical}");

            var exists = await ScalarAsync<int?>(con, "SELECT DB_ID(@db)", token, ("@db", db));

            var sql = new StringBuilder();
            if (exists.HasValue)
                sql.AppendLine($"ALTER DATABASE {QuoteName(db)} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
            sql.Append($"RESTORE DATABASE {QuoteName(db)} FROM DISK = {QuoteString(_testBackupPath)} WITH REPLACE, RECOVERY, STATS = 5");
            foreach (var m in moves)
                sql.Append($", MOVE {QuoteString(m.Logical)} TO {QuoteString(m.Physical)}");
            sql.Append(';');
            return sql.ToString();
        }

        private async Task TrySetMultiUserAsync(string db)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                using var con = await OpenAsync(_target, "master", cts.Token);
                var state = await ScalarAsync<string>(con, "SELECT user_access_desc + '/' + state_desc FROM sys.databases WHERE name = @db", cts.Token, ("@db", db));
                if (state != null && state.StartsWith("SINGLE_USER/ONLINE", StringComparison.Ordinal))
                {
                    using var cmd = new SqlCommand($"ALTER DATABASE {QuoteName(db)} SET MULTI_USER WITH ROLLBACK IMMEDIATE", con) { CommandTimeout = 60 };
                    await cmd.ExecuteNonQueryAsync(cts.Token);
                    Log($"Database '{db}' set back to MULTI_USER");
                }
            }
            catch (Exception ex)
            {
                Log($"WARNING: could not set '{db}' to MULTI_USER: {ex.Message}");
            }
        }

        /// <summary>
        /// Re-links database users that came from prod to this server's logins. Users are matched to logins by
        /// SID, and a SQL login has a different SID on each server, so after a restore they are orphaned.
        /// </summary>
        private async Task RelinkUsersAsync(CancellationToken token)
        {
            var sql = BuildRelinkSql(_relinks, _pair.RelinkUsersAsDbOwner);
            Log(sql);

            using var con = await OpenAsync(_target, _target.db, token);
            con.InfoMessage += (s, e) =>
            {
                foreach (SqlError err in e.Errors)
                    Log("  " + err.Message);
            };
            using var cmd = new SqlCommand(sql, con) { CommandTimeout = 120 };
            try
            {
                await cmd.ExecuteNonQueryAsync(token);
            }
            catch (SqlException ex) when (!token.IsCancellationRequested)
            {
                throw new TransferException(
                    "Re-linking users failed: " + ex.Message + Environment.NewLine +
                    "The database was restored. Note: a login that owns the database (e.g. the login used for the restore) " +
                    "is mapped to dbo and cannot also be linked to another user.", ex);
            }
        }

        /// <summary>
        /// T-SQL that links each user to its login (ALTER USER ... WITH LOGIN). With <paramref name="asDbOwner"/>,
        /// a missing user is created and every user is added to db_owner. Raises an error if a login is missing.
        /// </summary>
        public static string BuildRelinkSql(IEnumerable<UserLink> links, bool asDbOwner)
        {
            var sb = new StringBuilder();
            foreach (var l in links)
            {
                var user = QuoteName(l.User);
                var login = QuoteName(l.Login);
                string Msg(string text) => QuoteString(text);

                sb.AppendLine($"IF SUSER_ID({QuoteString(l.Login)}) IS NULL");
                sb.AppendLine($"    RAISERROR({Msg($"Login {login} does not exist on this server.".Replace("%", "%%"))}, 16, 1);");
                // A login that owns the database is mapped to dbo and cannot also be linked to another user.
                sb.AppendLine($"ELSE IF SUSER_SID({QuoteString(l.Login)}) = (SELECT sid FROM sys.database_principals WHERE name = N'dbo')");
                sb.AppendLine($"    PRINT {Msg($"Login {login} owns the database (mapped to dbo) and has full access; user {user} left as is.")};");
                sb.AppendLine($"ELSE IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = {QuoteString(l.User)})");
                sb.AppendLine("BEGIN");
                sb.AppendLine($"    ALTER USER {user} WITH LOGIN = {login};");
                sb.AppendLine($"    PRINT {Msg($"Re-linked user {user} to login {login}.")};");
                if (asDbOwner)
                {
                    sb.AppendLine($"    ALTER ROLE db_owner ADD MEMBER {user};");
                    sb.AppendLine($"    PRINT {Msg($"User {user} is db_owner.")};");
                }
                sb.AppendLine("END");
                if (asDbOwner)
                {
                    sb.AppendLine("ELSE");
                    sb.AppendLine("BEGIN");
                    sb.AppendLine($"    CREATE USER {user} FOR LOGIN {login};");
                    sb.AppendLine($"    ALTER ROLE db_owner ADD MEMBER {user};");
                    sb.AppendLine($"    PRINT {Msg($"Created user {user} for login {login} as db_owner.")};");
                    sb.AppendLine("END");
                }
                else
                {
                    sb.AppendLine($"ELSE");
                    sb.AppendLine($"    PRINT {Msg($"WARNING: user {user} does not exist in the database; nothing to re-link.")};");
                }
                sb.AppendLine();
            }
            return sb.ToString().TrimEnd();
        }

        private async Task RunScriptAsync(CancellationToken token)
        {
            var batches = SqlBatchSplitter.Split(_scriptText);
            using var con = await OpenAsync(_target, _target.db, token);
            con.InfoMessage += (s, e) =>
            {
                foreach (SqlError err in e.Errors)
                    Log("  " + err.Message);
            };

            for (int i = 0; i < batches.Count; i++)
            {
                var batch = batches[i];
                Detail(TransferStep.Script, $"Batch {i + 1} of {batches.Count} (line {batch.StartLine + 1})", 100.0 * i / Math.Max(1, batches.Count));
                for (int rep = 0; rep < Math.Max(1, batch.RepeatCount); rep++)
                {
                    using var cmd = new SqlCommand(batch.Text, con) { CommandTimeout = 0 };
                    try
                    {
                        int rows = await cmd.ExecuteNonQueryAsync(token);
                        Log($"Batch {i + 1}/{batches.Count} OK" + (rows >= 0 ? $" ({rows} rows affected)" : ""));
                    }
                    catch (SqlException ex) when (!token.IsCancellationRequested)
                    {
                        var lines = ex.Errors.Cast<SqlError>()
                            .Select(e => $"  Msg {e.Number}, Level {e.Class}, script line {batch.StartLine + Math.Max(1, e.LineNumber)}: {e.Message}");
                        throw new TransferException(
                            $"Post-restore script failed in batch {i + 1} (starting at line {batch.StartLine + 1}):" + Environment.NewLine +
                            string.Join(Environment.NewLine, lines) + Environment.NewLine +
                            "The database was restored; remaining batches were not run.", ex);
                    }
                }
            }
        }

        private async Task CleanupAsync()
        {
            _currentStep = TransferStep.Cleanup;
            Report(new TransferEvent { Step = TransferStep.Cleanup, Status = StepStatus.Running, Percent = 0, Detail = "" });
            Log("--- Cleanup ---");
            bool warnings = false;

            // Prod: always delete the backup.
            if (_prodBackupMayExist)
            {
                Detail(TransferStep.Cleanup, $"Deleting backup on {_sourceHost}", 10);
                warnings |= !await DeleteRemoteAsync(_pair.SourceSsh, _sourceHost, _prodBackupPath, "prod");
            }

            // Test: keep a complete backup if the restore did not succeed, so it can be restored by hand.
            if (_uploadStarted)
            {
                if (_uploadComplete && !_restoreSucceeded)
                {
                    if (TestRun)
                    {
                        Log($"KEEPING backup on test server so the printed restore command can be run: {_targetHost}:{_testBackupPath}");
                        Log("Delete it manually if you do not restore it.");
                    }
                    else
                    {
                        Log($"KEEPING backup on test server for manual restore: {_targetHost}:{_testBackupPath}");
                        warnings = true;
                    }
                }
                else
                {
                    Detail(TransferStep.Cleanup, $"Deleting backup on {_targetHost}", 50);
                    warnings |= !await DeleteRemoteAsync(_pair.TargetSsh, _targetHost, _testBackupPath, "test");
                }
            }

            // Local: keep it only if it is the last complete copy (it never made it to the test server).
            if (_localBackupPath != null && File.Exists(_localBackupPath))
            {
                if (_downloadComplete && !_uploadComplete)
                {
                    Log($"KEEPING local backup (it never reached the test server): {_localBackupPath}");
                    warnings = true;
                }
                else
                {
                    try
                    {
                        File.Delete(_localBackupPath);
                        Log($"Deleted local file {_localBackupPath}");
                    }
                    catch (Exception ex)
                    {
                        Log($"WARNING: could not delete local file {_localBackupPath}: {ex.Message}");
                        warnings = true;
                    }
                }
            }

            Report(new TransferEvent { Step = TransferStep.Cleanup, Status = warnings ? StepStatus.Warning : StepStatus.Succeeded, Percent = 100, Detail = warnings ? "See log" : "" });
        }

        private async Task<bool> DeleteRemoteAsync(SshEndpoint ep, string host, string path, string side)
        {
            try
            {
                using var cts = new CancellationTokenSource(CleanupTimeout);
                var r = await _ssh.DeleteRemoteFileAsync(host, ep.User, ep.Port, path, cts.Token);
                if (r.Success)
                {
                    Log($"Deleted {side} file {host}:{path}");
                    return true;
                }
                Log($"WARNING: could not delete {side} file {host}:{path} ({r.Describe()}). Delete it manually.");
            }
            catch (Exception ex)
            {
                Log($"WARNING: could not delete {side} file {host}:{path}: {ex.Message}. Delete it manually.");
            }
            return false;
        }

        #endregion

        #region SQL helpers

        internal static string BuildConnectionString(Connection c, string database)
        {
            var csb = new SqlConnectionStringBuilder
            {
                DataSource = c.server,
                InitialCatalog = database,
                ApplicationName = "SqlStudio DB Transfer",
                ConnectTimeout = 30,
            };
            if (c.integrated_security || string.IsNullOrEmpty(c.password))
            {
                csb.IntegratedSecurity = true;
            }
            else
            {
                csb.UserID = c.user;
                csb.Password = c.password;
            }
            return csb.ConnectionString;
        }

        private static async Task<SqlConnection> OpenAsync(Connection c, string database, CancellationToken token)
        {
            var con = new SqlConnection(BuildConnectionString(c, database));
            try
            {
                await con.OpenAsync(token);
            }
            catch (SqlException ex)
            {
                con.Dispose();
                throw new TransferException($"Cannot connect to {c.server} ({database}): {ex.Message}", ex);
            }
            return con;
        }

        private static async Task<T> ScalarAsync<T>(SqlConnection con, string sql, CancellationToken token, params (string Name, object Value)[] parameters)
        {
            using var cmd = new SqlCommand(sql, con) { CommandTimeout = 60 };
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
            var result = await cmd.ExecuteScalarAsync(token);
            if (result == null || result == DBNull.Value)
                return default;
            var t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            return (T)Convert.ChangeType(result, t, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Runs BACKUP/RESTORE and turns the WITH STATS messages into progress. FireInfoMessageEventOnUserErrors
        /// makes messages arrive as they are sent instead of when the command completes, but it also routes
        /// errors of severity 16 and below to InfoMessage, so those are collected and thrown afterwards.
        /// </summary>
        private async Task ExecuteWithStatsAsync(SqlConnection con, string sql, TransferStep step, CancellationToken token)
        {
            var errors = new List<string>();
            var sw = Stopwatch.StartNew();
            void Handler(object s, SqlInfoMessageEventArgs e)
            {
                foreach (SqlError err in e.Errors)
                {
                    if (err.Class > 10)
                    {
                        lock (errors)
                            errors.Add($"Msg {err.Number}, Level {err.Class}: {err.Message}");
                        Log("  ERROR: " + err.Message);
                    }
                    else if (err.Number == PercentProcessedMessage && TryLeadingInt(err.Message, out int pct))
                    {
                        var eta = pct > 0 ? TimeSpan.FromSeconds(sw.Elapsed.TotalSeconds * (100 - pct) / pct) : (TimeSpan?)null;
                        Detail(step, $"{pct}% processed" + (eta.HasValue && pct < 100 ? $", about {FormatElapsed(eta.Value)} left" : ""), pct);
                        Log("  " + err.Message);
                    }
                    else
                    {
                        Log("  " + err.Message);
                    }
                }
            }

            con.FireInfoMessageEventOnUserErrors = true;
            con.InfoMessage += Handler;
            try
            {
                using var cmd = new SqlCommand(sql, con) { CommandTimeout = 0 };
                await cmd.ExecuteNonQueryAsync(token);
            }
            finally
            {
                con.InfoMessage -= Handler;
                con.FireInfoMessageEventOnUserErrors = false;
            }

            lock (errors)
            {
                if (errors.Count > 0)
                    throw new TransferException($"{step} failed:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }
        }

        public class BackupFile
        {
            public string LogicalName { get; set; }

            /// <summary>D = data, L = log, S = FILESTREAM / memory-optimized container, F = full-text catalog.</summary>
            public string Type { get; set; }

            public long FileId { get; set; }
        }

        public class MoveClause
        {
            public string Logical { get; set; }
            public string Physical { get; set; }
        }

        private async Task<List<BackupFile>> ReadFileListAsync(SqlConnection con, CancellationToken token)
        {
            var files = new List<BackupFile>();
            using var cmd = new SqlCommand($"RESTORE FILELISTONLY FROM DISK = {QuoteString(_testBackupPath)}", con) { CommandTimeout = 0 };
            using var reader = await cmd.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                files.Add(new BackupFile
                {
                    LogicalName = Convert.ToString(reader["LogicalName"], CultureInfo.InvariantCulture),
                    Type = Convert.ToString(reader["Type"], CultureInfo.InvariantCulture)?.Trim(),
                    FileId = Convert.ToInt64(reader["FileId"], CultureInfo.InvariantCulture),
                });
            }
            if (files.Count == 0)
                throw new TransferException("RESTORE FILELISTONLY returned no files; the backup file is not usable.");
            return files;
        }

        /// <summary>
        /// Places every file of the backup in the test server's default folders, named after the target database:
        /// first data file -> db.mdf, other data files -> db_logical.ndf, first log -> db_log.ldf.
        /// </summary>
        public static List<MoveClause> BuildMoveClauses(IEnumerable<BackupFile> files, string targetDb, string dataFolder, string logFolder)
        {
            string Folder(string f) => f.EndsWith("\\", StringComparison.Ordinal) ? f : f + "\\";
            var dataDir = Folder(dataFolder.Trim());
            var logDir = Folder(logFolder.Trim());
            var baseName = SafeFileName(targetDb);

            var result = new List<MoveClause>();
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            bool firstData = true, firstLog = true;

            foreach (var f in files.OrderBy(f => f.FileId))
            {
                string physical;
                switch (f.Type?.ToUpperInvariant())
                {
                    case "D":
                        physical = dataDir + baseName + (firstData ? ".mdf" : "_" + SafeFileName(f.LogicalName) + ".ndf");
                        firstData = false;
                        break;
                    case "L":
                        physical = logDir + baseName + (firstLog ? "_log.ldf" : "_" + SafeFileName(f.LogicalName) + ".ldf");
                        firstLog = false;
                        break;
                    default:
                        // FILESTREAM / in-memory containers and full-text catalogs are directories.
                        physical = dataDir + baseName + "_" + SafeFileName(f.LogicalName);
                        break;
                }

                // Two logical names that sanitize to the same string must not collide.
                var candidate = physical;
                for (int n = 2; !used.Add(candidate); n++)
                {
                    int dot = physical.LastIndexOf('.');
                    bool hasExt = dot > physical.LastIndexOf('\\');
                    candidate = hasExt ? physical.Substring(0, dot) + "_" + n + physical.Substring(dot) : physical + "_" + n;
                }

                result.Add(new MoveClause { Logical = f.LogicalName, Physical = candidate });
            }
            return result;
        }

        #endregion

        #region Formatting and misc

        private void Detail(TransferStep step, string detail, double? percent) =>
            Report(new TransferEvent { Step = step, Detail = detail, Percent = percent });

        private void Report(TransferEvent e) => _progress?.Report(e);

        private void Log(string line)
        {
            var stamped = $"{DateTime.Now:HH:mm:ss}  {line}";
            lock (_logLock)
            {
                try
                {
                    _logWriter?.WriteLine(stamped);
                }
                catch
                {
                    // Logging must never break the transfer.
                }
            }
            Report(new TransferEvent { Step = _currentStep, LogLine = stamped });
        }

        private async Task PollAsync(TransferStep step, long total, Stopwatch sw, Func<Task<long>> getSize, TimeSpan interval, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(interval, token);
                long size;
                try
                {
                    size = await getSize();
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    continue;
                }
                if (size < 0)
                    continue;
                double pct = total > 0 ? Math.Min(100.0, 100.0 * size / total) : 0;
                var rate = FormatRate(size, sw.Elapsed);
                string eta = "";
                if (size > 0 && size < total)
                {
                    var remaining = TimeSpan.FromSeconds(sw.Elapsed.TotalSeconds * (total - size) / size);
                    eta = $", about {FormatElapsed(remaining)} left";
                }
                Detail(step, $"{FormatBytes(size)} of {FormatBytes(total)} ({rate}{eta})", pct);
            }
        }

        private static async Task IgnoreCancel(Task t)
        {
            try
            {
                await t;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static bool TryLeadingInt(string s, out int value)
        {
            value = 0;
            int i = 0;
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
            int start = i;
            while (i < s.Length && char.IsDigit(s[i])) i++;
            return i > start && int.TryParse(s.Substring(start, i - start), NumberStyles.None, CultureInfo.InvariantCulture, out value);
        }

        private static string Describe(SshEndpoint ep, string host) =>
            (string.IsNullOrWhiteSpace(ep.User) ? "" : ep.User.Trim() + "@") + host + (ep.Port != 22 ? ":" + ep.Port : "") + " " + ep.Folder;

        /// <summary>[name] with ] escaped, like QUOTENAME.</summary>
        public static string QuoteName(string name) => "[" + name.Replace("]", "]]") + "]";

        /// <summary>N'value' with ' escaped.</summary>
        public static string QuoteString(string value) => "N'" + value.Replace("'", "''") + "'";

        /// <summary>Keeps letters, digits, '-', '_' and '.' so the name is safe on disk and in remote shell commands.</summary>
        public static string SafeFileName(string name)
        {
            var sb = new StringBuilder();
            foreach (var ch in name ?? "")
                sb.Append(char.IsLetterOrDigit(ch) && ch < 128 || ch == '-' || ch == '_' || ch == '.' ? ch : '_');
            var s = sb.ToString().Trim('.');
            return s.Length == 0 ? "db" : s;
        }

        private static string WindowsDirectoryName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;
            int i = path.LastIndexOf('\\');
            return i > 0 ? path.Substring(0, i + 1) : null;
        }

        public static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double v = bytes;
            int u = 0;
            while (v >= 1024 && u < units.Length - 1)
            {
                v /= 1024;
                u++;
            }
            return u == 0 ? $"{bytes} B" : v.ToString("0.0", CultureInfo.InvariantCulture) + " " + units[u];
        }

        private static string FormatRate(long bytes, TimeSpan elapsed) =>
            elapsed.TotalSeconds < 0.5 ? "-" : FormatBytes((long)(bytes / elapsed.TotalSeconds)) + "/s";

        public static string FormatElapsed(TimeSpan t) =>
            t.TotalHours >= 1 ? $"{(int)t.TotalHours}h {t.Minutes:00}m {t.Seconds:00}s"
            : t.TotalMinutes >= 1 ? $"{t.Minutes}m {t.Seconds:00}s"
            : $"{t.Seconds}s";

        private static string FirstLine(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            int i = s.IndexOfAny(new[] { '\r', '\n' });
            return i < 0 ? s : s.Substring(0, i);
        }

        #endregion
    }
}
