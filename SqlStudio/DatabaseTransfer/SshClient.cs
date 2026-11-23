using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStudio.DatabaseTransfer
{
    public class SshCommandResult
    {
        public int ExitCode { get; set; }
        public string StdOut { get; set; } = "";
        public string StdErr { get; set; } = "";
        public bool Success => ExitCode == 0;

        public string Describe() =>
            $"exit code {ExitCode}" + (string.IsNullOrWhiteSpace(StdErr) ? "" : ": " + StdErr.Trim());
    }

    /// <summary>
    /// Thin wrapper around the Windows OpenSSH client (ssh.exe/scp.exe). Key based auth is assumed;
    /// BatchMode makes the tools fail immediately instead of hanging on a password or host key prompt.
    /// Remote commands are run through powershell.exe, which works whether the server's default ssh
    /// shell is cmd or PowerShell (see <see cref="PowerShellCommand"/>).
    /// </summary>
    public class SshClient
    {
        private readonly string _sshExe;
        private readonly string _scpExe;

        public SshClient()
        {
            _sshExe = FindTool("ssh.exe");
            _scpExe = FindTool("scp.exe");
        }

        public bool ToolsAvailable => _sshExe != null && _scpExe != null;

        public string ToolsDescription => $"ssh: {_sshExe ?? "NOT FOUND"}, scp: {_scpExe ?? "NOT FOUND"}";

        private static string FindTool(string exe)
        {
            var candidates = new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "OpenSSH", exe),
                // A 32-bit process sees System32 redirected; Sysnative reaches the real one.
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Sysnative", "OpenSSH", exe),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "OpenSSH", exe),
            };
            foreach (var dir in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
            {
                if (!string.IsNullOrWhiteSpace(dir))
                    candidates.Add(Path.Combine(dir.Trim(), exe));
            }

            foreach (var c in candidates)
            {
                try
                {
                    if (File.Exists(c))
                        return c;
                }
                catch
                {
                    // Bad PATH entry
                }
            }
            return null;
        }

        /// <summary>"D:\Backups\x.bak" -> "/D:/Backups/x.bak", the form Windows OpenSSH's scp/sftp accepts.</summary>
        public static string ToScpPath(string windowsPath)
        {
            var p = windowsPath.Trim().Replace('\\', '/');
            return p.StartsWith("/", StringComparison.Ordinal) ? p : "/" + p;
        }

        /// <summary>Joins a Windows folder and file name using backslashes, regardless of the local OS conventions.</summary>
        public static string CombineWindowsPath(string folder, string fileName)
        {
            var f = folder.Trim().Replace('/', '\\').TrimEnd('\\');
            return f + "\\" + fileName;
        }

        private static string Destination(string host, string user) =>
            string.IsNullOrWhiteSpace(user) ? host : user.Trim() + "@" + host;

        private static IEnumerable<string> CommonOptions()
        {
            yield return "-o"; yield return "BatchMode=yes";
            yield return "-o"; yield return "ConnectTimeout=20";
            yield return "-o"; yield return "ServerAliveInterval=15";
            yield return "-o"; yield return "ServerAliveCountMax=4";
        }

        /// <summary>
        /// Wraps a PowerShell script as an ssh remote command. The script is passed with -EncodedCommand
        /// (base64 of UTF-16LE), which contains no characters that the server's login shell (cmd or
        /// PowerShell) would interpret, so quoting, braces, '$' and '%' all survive unchanged.
        /// </summary>
        public static string PowerShellCommand(string script)
        {
            var full = "$ProgressPreference = 'SilentlyContinue'; $ErrorActionPreference = 'Stop'; " + script;
            var encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(full));
            return "powershell.exe -NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + encoded;
        }

        /// <summary>Single-quoted PowerShell string literal.</summary>
        public static string PsLiteral(string value) => "'" + value.Replace("'", "''") + "'";

        public Task<SshCommandResult> RunRemoteAsync(string host, string user, int port, string command, CancellationToken token)
        {
            var args = new List<string>(CommonOptions()) { "-p", port.ToString(CultureInfo.InvariantCulture), Destination(host, user), command };
            return RunProcessAsync(_sshExe, args, token);
        }

        public Task<SshCommandResult> RunRemotePowerShellAsync(string host, string user, int port, string script, CancellationToken token) =>
            RunRemoteAsync(host, user, port, PowerShellCommand(script), token);

        /// <summary>Size of a remote file in bytes, or -1 if it does not exist / cannot be read.</summary>
        public async Task<long> GetRemoteFileSizeAsync(string host, string user, int port, string windowsPath, CancellationToken token)
        {
            var lit = PsLiteral(windowsPath);
            var script = $"if (Test-Path -LiteralPath {lit} -PathType Leaf) {{ (Get-Item -LiteralPath {lit}).Length }} else {{ -1 }}";
            var r = await RunRemotePowerShellAsync(host, user, port, script, token).ConfigureAwait(false);
            if (!r.Success)
                return -1;
            foreach (var line in r.StdOut.Split('\n'))
            {
                if (long.TryParse(line.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
                    return size;
            }
            return -1;
        }

        /// <summary>Deletes a remote file. Succeeds if the file does not exist (nothing to clean up).</summary>
        public Task<SshCommandResult> DeleteRemoteFileAsync(string host, string user, int port, string windowsPath, CancellationToken token)
        {
            var lit = PsLiteral(windowsPath);
            var script = $"if (Test-Path -LiteralPath {lit}) {{ Remove-Item -LiteralPath {lit} -Force -ErrorAction Stop }}";
            return RunRemotePowerShellAsync(host, user, port, script, token);
        }

        /// <summary>Checks that the folder exists remotely (and that ssh login works at all).</summary>
        public Task<SshCommandResult> CheckRemoteFolderAsync(string host, string user, int port, string windowsFolder, CancellationToken token)
        {
            var script = $"if (-not (Test-Path -LiteralPath {PsLiteral(windowsFolder)} -PathType Container)) {{ [Console]::Error.WriteLine('Folder not found'); exit 2 }}";
            return RunRemotePowerShellAsync(host, user, port, script, token);
        }

        public Task<SshCommandResult> DownloadAsync(string host, string user, int port, string remoteWindowsPath, string localPath, CancellationToken token)
        {
            var args = new List<string>(CommonOptions()) { "-B", "-P", port.ToString(CultureInfo.InvariantCulture),
                Destination(host, user) + ":" + ToScpPath(remoteWindowsPath), localPath };
            return RunProcessAsync(_scpExe, args, token);
        }

        public Task<SshCommandResult> UploadAsync(string host, string user, int port, string localPath, string remoteWindowsPath, CancellationToken token)
        {
            var args = new List<string>(CommonOptions()) { "-B", "-P", port.ToString(CultureInfo.InvariantCulture),
                localPath, Destination(host, user) + ":" + ToScpPath(remoteWindowsPath) };
            return RunProcessAsync(_scpExe, args, token);
        }

        private static async Task<SshCommandResult> RunProcessAsync(string exe, IEnumerable<string> args, CancellationToken token)
        {
            if (exe == null)
                throw new InvalidOperationException("OpenSSH client (ssh.exe/scp.exe) was not found. Install the Windows 'OpenSSH Client' optional feature.");

            var psi = new ProcessStartInfo(exe)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            foreach (var a in args)
                psi.ArgumentList.Add(a);

            using var process = new Process { StartInfo = psi };
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            process.OutputDataReceived += (s, e) => { if (e.Data != null) lock (stdout) stdout.AppendLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (stderr) stderr.AppendLine(e.Data); };

            process.Start();
            process.StandardInput.Close();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            try
            {
                await process.WaitForExitAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // Already gone
                }
                throw;
            }

            // Make sure the async readers have drained.
            process.WaitForExit();

            lock (stdout) lock (stderr)
                return new SshCommandResult { ExitCode = process.ExitCode, StdOut = stdout.ToString(), StdErr = stderr.ToString() };
        }
    }
}
