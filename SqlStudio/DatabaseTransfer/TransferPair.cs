using System.Collections.Generic;

namespace SqlStudio.DatabaseTransfer
{
    /// <summary>
    /// ssh/scp settings for one side of a transfer. The folder is a local Windows path on that server
    /// (e.g. D:\Backups); SQL Server writes/reads the backup there and scp copies to/from it.
    /// </summary>
    public class SshEndpoint
    {
        /// <summary>ssh host. Empty = derived from the SQL connection's server name.</summary>
        public string Host { get; set; } = "";

        /// <summary>ssh user. Empty = let ~/.ssh/config (or the current Windows user) decide.</summary>
        public string User { get; set; } = "";

        public int Port { get; set; } = 22;

        /// <summary>Backup folder on the server, as a Windows path, e.g. D:\Backups.</summary>
        public string Folder { get; set; } = "";

        public SshEndpoint Clone() => (SshEndpoint)MemberwiseClone();
    }

    /// <summary>A preconfigured prod -> test database transfer.</summary>
    public class TransferPair
    {
        public string Name { get; set; } = "New transfer";

        /// <summary>p_key of the saved production connection (backup source).</summary>
        public long SourceConnectionId { get; set; }

        /// <summary>p_key of the saved test connection (restore target).</summary>
        public long TargetConnectionId { get; set; }

        /// <summary>Database to back up. Empty = the source connection's database.</summary>
        public string SourceDatabase { get; set; } = "";

        /// <summary>Database to restore into. Empty = the target connection's database.</summary>
        public string TargetDatabase { get; set; } = "";

        public SshEndpoint SourceSsh { get; set; } = new SshEndpoint();
        public SshEndpoint TargetSsh { get; set; } = new SshEndpoint();

        /// <summary>Optional .sql file run against the restored test database. Empty = none.</summary>
        public string PostRestoreScriptPath { get; set; } = "";

        /// <summary>
        /// Database users to re-link to the test server's logins after restore, separated by comma, semicolon
        /// or newline. "apx" links user apx to login apx; "app=DOMAIN\svc_app" links user app to that login.
        /// Needed because users are matched to logins by SID, and SQL logins have different SIDs on each server.
        /// </summary>
        public string RelinkUsers { get; set; } = "";

        /// <summary>Also make the re-linked users members of db_owner (creating the user if it does not exist).</summary>
        public bool RelinkUsersAsDbOwner { get; set; }

        public TransferPair Clone()
        {
            var c = (TransferPair)MemberwiseClone();
            c.SourceSsh = (SourceSsh ?? new SshEndpoint()).Clone();
            c.TargetSsh = (TargetSsh ?? new SshEndpoint()).Clone();
            return c;
        }

        public override string ToString() => Name;
    }

    /// <summary>One database user -> server login mapping from <see cref="TransferPair.RelinkUsers"/>.</summary>
    public class UserLink
    {
        public string User { get; set; }
        public string Login { get; set; }

        public override string ToString() => User == Login ? User : User + "=" + Login;

        /// <summary>Parses "apx, app=DOMAIN\svc_app". Empty entries are ignored; "user=" or "=login" keep the empty side.</summary>
        public static List<UserLink> Parse(string text)
        {
            var result = new List<UserLink>();
            foreach (var raw in (text ?? "").Split(new[] { ',', ';', '\n', '\r' }))
            {
                var entry = raw.Trim();
                if (entry.Length == 0)
                    continue;
                int eq = entry.IndexOf('=');
                if (eq < 0)
                    result.Add(new UserLink { User = entry, Login = entry });
                else
                    result.Add(new UserLink { User = entry.Substring(0, eq).Trim(), Login = entry.Substring(eq + 1).Trim() });
            }
            return result;
        }
    }

    public class TransferSettings
    {
        /// <summary>Folder on this machine where the backup is staged between scp hops and run logs are written.</summary>
        public string LocalFolder { get; set; } = "";

        public List<TransferPair> Pairs { get; set; } = new List<TransferPair>();
    }
}
