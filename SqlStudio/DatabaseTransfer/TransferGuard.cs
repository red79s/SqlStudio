using System;
using System.Collections.Generic;
using System.Linq;
using CfgDataStore;

namespace SqlStudio.DatabaseTransfer
{
    /// <summary>
    /// Enforces that a transfer only ever goes production -> test and never writes to a production server.
    /// Checked when a pair is saved and again immediately before a run, since connection flags can change.
    /// </summary>
    public static class TransferGuard
    {
        public const string SqlServerProvider = "Sql Server";

        /// <summary>Characters rejected in remote folders: they would be interpreted by the remote shell.</summary>
        private static readonly char[] UnsafeFolderChars = { '"', '\'', '$', '%', '`', ';', '&', '|', '<', '>', '\r', '\n' };

        /// <summary>
        /// Returns every reason the pair is not allowed. Empty list = OK.
        /// </summary>
        public static List<string> Validate(TransferPair pair, IReadOnlyList<Connection> connections)
        {
            var errors = new List<string>();
            if (pair == null)
            {
                errors.Add("No transfer selected.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(pair.Name))
                errors.Add("The transfer needs a name.");

            var source = ResolveSource(pair, connections);
            var target = ResolveTarget(pair, connections);

            if (source == null)
                errors.Add("Source (production) connection is not set or no longer exists.");
            if (target == null)
                errors.Add("Target (test) connection is not set or no longer exists.");

            if (source != null)
            {
                if (source.provider != SqlServerProvider)
                    errors.Add($"Source connection '{source.description}' is not a Sql Server connection.");
                if (source.IsProduction != true)
                    errors.Add($"Source connection '{source.description}' is not marked as a production database. Transfers must go from production to test.");
                if (string.IsNullOrWhiteSpace(source.db))
                    errors.Add($"No source database: set one on the transfer or on connection '{source.description}'.");
            }

            if (target != null)
            {
                if (target.provider != SqlServerProvider)
                    errors.Add($"Target connection '{target.description}' is not a Sql Server connection.");
                if (target.IsProduction == true)
                    errors.Add($"Target connection '{target.description}' is marked as a production database. Restoring onto production is not allowed.");
                if (string.IsNullOrWhiteSpace(target.db))
                    errors.Add($"No target database: set one on the transfer or on connection '{target.description}'.");
                else if (IsSystemDatabase(target.db))
                    errors.Add($"Target database '{target.db}' is a system database.");

                var targetInstance = NormalizeInstance(target.server);
                var prodHit = connections.FirstOrDefault(c => c.IsProduction == true
                    && c.provider == SqlServerProvider
                    && NormalizeInstance(c.server) == targetInstance);
                if (prodHit != null)
                    errors.Add($"Target server '{target.server}' hosts the production connection '{prodHit.description}'. Restoring onto a production server is not allowed.");

                if (source != null && NormalizeInstance(source.server) == targetInstance)
                    errors.Add("Source and target are on the same SQL Server instance.");
            }

            ValidateEndpoint("Source", pair.SourceSsh, source, errors);
            ValidateEndpoint("Target", pair.TargetSsh, target, errors);

            foreach (var link in UserLink.Parse(pair.RelinkUsers))
            {
                if (link.User.Length == 0 || link.Login.Length == 0)
                    errors.Add($"Re-link users: '{link}' needs both a user and a login (user=login).");
                else if (IsReservedUser(link.User))
                    errors.Add($"Re-link users: '{link.User}' is a built-in user and cannot be re-linked.");
            }

            return errors;
        }

        private static bool IsReservedUser(string user)
        {
            var u = user.ToLowerInvariant();
            return u == "dbo" || u == "guest" || u == "sys" || u == "information_schema";
        }

        private static void ValidateEndpoint(string side, SshEndpoint ep, Connection con, List<string> errors)
        {
            if (ep == null)
            {
                errors.Add($"{side} ssh settings are missing.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ResolveSshHost(ep, con)))
                errors.Add($"{side} ssh host is not set.");
            if (ep.Port <= 0 || ep.Port > 65535)
                errors.Add($"{side} ssh port is invalid.");
            if (string.IsNullOrWhiteSpace(ep.Folder))
                errors.Add($"{side} backup folder is not set.");
            else if (!IsLocalWindowsPath(ep.Folder))
                errors.Add($"{side} backup folder must be a local Windows path on the server, like D:\\Backups.");
            else if (ep.Folder.IndexOfAny(UnsafeFolderChars) >= 0)
                errors.Add($"{side} backup folder contains characters that are not allowed (quotes, $, %, etc.).");
        }

        /// <summary>The pair's source connection with the pair's database override applied (a copy), or null.</summary>
        public static Connection ResolveSource(TransferPair pair, IEnumerable<Connection> connections) =>
            WithDatabase(connections.FirstOrDefault(c => c.p_key == pair.SourceConnectionId), pair.SourceDatabase);

        /// <summary>The pair's target connection with the pair's database override applied (a copy), or null.</summary>
        public static Connection ResolveTarget(TransferPair pair, IEnumerable<Connection> connections) =>
            WithDatabase(connections.FirstOrDefault(c => c.p_key == pair.TargetConnectionId), pair.TargetDatabase);

        /// <summary>
        /// Copy of the connection using <paramref name="database"/> instead of its default database when one is given.
        /// A copy, so the saved connection (an EF entity) is never modified.
        /// </summary>
        public static Connection WithDatabase(Connection c, string database)
        {
            if (c == null)
                return null;
            return new Connection
            {
                p_key = c.p_key,
                provider = c.provider,
                server = c.server,
                db = string.IsNullOrWhiteSpace(database) ? c.db : database.Trim(),
                user = c.user,
                password = c.password,
                integrated_security = c.integrated_security,
                default_connection = c.default_connection,
                IsProduction = c.IsProduction,
                description = c.description,
            };
        }

        /// <summary>True for paths like "D:\Backups" or "D:/Backups".</summary>
        public static bool IsLocalWindowsPath(string path)
        {
            path = path?.Trim();
            return !string.IsNullOrEmpty(path)
                && path.Length >= 2
                && char.IsLetter(path[0])
                && path[1] == ':'
                && (path.Length == 2 || path[2] == '\\' || path[2] == '/');
        }

        public static bool IsSystemDatabase(string db)
        {
            var d = db?.Trim().ToLowerInvariant();
            return d == "master" || d == "model" || d == "msdb" || d == "tempdb" || d == "distribution";
        }

        /// <summary>The ssh host to use: the configured one, or the machine part of the SQL server name.</summary>
        public static string ResolveSshHost(SshEndpoint ep, Connection con)
        {
            if (!string.IsNullOrWhiteSpace(ep?.Host))
                return ep.Host.Trim();
            return con == null ? "" : HostFromServerName(con.server);
        }

        /// <summary>
        /// Machine name from a SQL Server data source: "tcp:PROD01\INST,1433" -> "PROD01".
        /// Local aliases (".", "(local)") map to "localhost".
        /// </summary>
        public static string HostFromServerName(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
                return "";

            var s = server.Trim();
            int colon = s.IndexOf(':');
            if (colon > 0 && colon <= 4 && !s.StartsWith("[", StringComparison.Ordinal))
            {
                // Protocol prefix such as tcp:, np:, lpc:
                var prefix = s.Substring(0, colon).ToLowerInvariant();
                if (prefix == "tcp" || prefix == "np" || prefix == "lpc" || prefix == "admin")
                    s = s.Substring(colon + 1);
            }

            int cut = s.IndexOfAny(new[] { '\\', ',' });
            if (cut >= 0)
                s = s.Substring(0, cut);

            s = s.Trim();
            if (s == "." || s.Equals("(local)", StringComparison.OrdinalIgnoreCase) || s.Equals("(localdb)", StringComparison.OrdinalIgnoreCase))
                return "localhost";
            return s;
        }

        /// <summary>
        /// Canonical "host\instance" form used to compare servers: "tcp:Prod01,1433" and "prod01" are equal.
        /// </summary>
        public static string NormalizeInstance(string server)
        {
            var host = HostFromServerName(server).ToLowerInvariant();
            if (host == "127.0.0.1" || host == "::1")
                host = "localhost";
            if (host == Environment.MachineName.ToLowerInvariant())
                host = "localhost";

            // Strip the domain part so "prod01" and "prod01.corp.local" compare equal.
            if (!System.Net.IPAddress.TryParse(host, out _))
            {
                int dot = host.IndexOf('.');
                if (dot > 0)
                    host = host.Substring(0, dot);
            }

            var instance = "";
            var s = server?.Trim() ?? "";
            int bs = s.IndexOf('\\');
            if (bs >= 0)
            {
                instance = s.Substring(bs + 1);
                int comma = instance.IndexOf(',');
                if (comma >= 0)
                    instance = instance.Substring(0, comma);
                instance = instance.Trim().ToLowerInvariant();
                if (instance == "mssqlserver")
                    instance = "";
            }

            return instance.Length == 0 ? host : host + "\\" + instance;
        }
    }
}
