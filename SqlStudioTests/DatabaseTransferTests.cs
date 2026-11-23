using System;
using System.Collections.Generic;
using System.Linq;
using CfgDataStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlStudio.DatabaseTransfer;

namespace SqlStudioTests
{
    [TestClass]
    public class DatabaseTransferTests
    {
        private static Connection Con(long id, string server, string db, bool prod, string provider = "Sql Server") =>
            new Connection { p_key = id, server = server, db = db, IsProduction = prod, provider = provider, description = "con" + id };

        private static TransferPair Pair(long source, long target) => new TransferPair
        {
            Name = "t",
            SourceConnectionId = source,
            TargetConnectionId = target,
            SourceSsh = new SshEndpoint { Folder = @"D:\Backups" },
            TargetSsh = new SshEndpoint { Folder = @"E:\Restore" },
        };

        [TestMethod]
        public void Validate_ProdToTest_IsAllowed()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "Sales_Test", false) };
            var errors = TransferGuard.Validate(Pair(1, 2), cons);
            Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
        }

        [TestMethod]
        public void Validate_TestToProd_IsRejected()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "Sales_Test", false) };
            var errors = TransferGuard.Validate(Pair(2, 1), cons);
            Assert.IsTrue(errors.Any(e => e.Contains("not marked as a production")));
            Assert.IsTrue(errors.Any(e => e.Contains("Restoring onto production is not allowed")));
        }

        [TestMethod]
        public void Validate_TargetOnServerHostingAnyProdConnection_IsRejected()
        {
            // Target itself is not flagged, but another prod connection lives on the same instance.
            var cons = new List<Connection>
            {
                Con(1, "PROD01", "Sales", true),
                Con(2, "TEST01", "Sales_Test", false),
                Con(3, "tcp:test01.corp.local,1433", "Billing", true),
            };
            var errors = TransferGuard.Validate(Pair(1, 2), cons);
            Assert.IsTrue(errors.Any(e => e.Contains("hosts the production connection")), string.Join("\n", errors));
        }

        [TestMethod]
        public void Validate_SameInstance_IsRejected()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "prod01", "Sales_Copy", false) };
            var errors = TransferGuard.Validate(Pair(1, 2), cons);
            Assert.IsTrue(errors.Any(e => e.Contains("hosts the production connection") || e.Contains("same SQL Server instance")));
        }

        [TestMethod]
        public void Validate_SystemDatabaseTarget_IsRejected()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "master", false) };
            Assert.IsTrue(TransferGuard.Validate(Pair(1, 2), cons).Any(e => e.Contains("system database")));
        }

        [TestMethod]
        public void Validate_NonSqlServer_IsRejected()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true, "PostgreSql"), Con(2, "TEST01", "Sales", false) };
            Assert.IsTrue(TransferGuard.Validate(Pair(1, 2), cons).Any(e => e.Contains("not a Sql Server")));
        }

        [TestMethod]
        public void Validate_MissingConnection_IsRejected()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true) };
            Assert.IsTrue(TransferGuard.Validate(Pair(1, 99), cons).Any(e => e.Contains("Target (test) connection is not set")));
        }

        [TestMethod]
        public void DatabaseOverride_IsUsedAndValidated()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "", false) };
            var p = Pair(1, 2);

            // Target connection has no default database and no override.
            Assert.IsTrue(TransferGuard.Validate(p, cons).Any(e => e.StartsWith("No target database")));

            p.SourceDatabase = " Billing ";
            p.TargetDatabase = "Billing_Test";
            Assert.AreEqual(0, TransferGuard.Validate(p, cons).Count);
            Assert.AreEqual("Billing", TransferGuard.ResolveSource(p, cons).db);
            Assert.AreEqual("Billing_Test", TransferGuard.ResolveTarget(p, cons).db);

            // The saved connections are not modified.
            Assert.AreEqual("Sales", cons[0].db);
            Assert.AreEqual("", cons[1].db);

            p.TargetDatabase = "msdb";
            Assert.IsTrue(TransferGuard.Validate(p, cons).Any(e => e.Contains("system database")));
        }

        [TestMethod]
        public void UserLink_Parse()
        {
            var links = UserLink.Parse(" apx, reporting = CORP\\svc_report ;\r\n\r\nbatch ");
            Assert.AreEqual(3, links.Count);
            Assert.AreEqual("apx", links[0].User);
            Assert.AreEqual("apx", links[0].Login);
            Assert.AreEqual("reporting", links[1].User);
            Assert.AreEqual("CORP\\svc_report", links[1].Login);
            Assert.AreEqual("batch", links[2].User);
            Assert.AreEqual(0, UserLink.Parse("  ").Count);
        }

        [TestMethod]
        public void Validate_RelinkUsers()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "Sales_Test", false) };
            var p = Pair(1, 2);
            p.RelinkUsers = "apx, app=CORP\\svc";
            Assert.AreEqual(0, TransferGuard.Validate(p, cons).Count);

            p.RelinkUsers = "dbo";
            Assert.IsTrue(TransferGuard.Validate(p, cons).Any(e => e.Contains("built-in user")));
            p.RelinkUsers = "apx=";
            Assert.IsTrue(TransferGuard.Validate(p, cons).Any(e => e.Contains("needs both a user and a login")));
        }

        [TestMethod]
        public void BuildRelinkSql()
        {
            var links = UserLink.Parse("apx, o'brien=CORP\\o]b");

            var plain = DatabaseTransferRunner.BuildRelinkSql(links, asDbOwner: false);
            StringAssert.Contains(plain, "ALTER USER [apx] WITH LOGIN = [apx];");
            StringAssert.Contains(plain, "ALTER USER [o'brien] WITH LOGIN = [CORP\\o]]b];");
            StringAssert.Contains(plain, "name = N'o''brien'");
            StringAssert.Contains(plain, "SUSER_ID(N'CORP\\o]b')");
            Assert.IsFalse(plain.Contains("db_owner"));
            Assert.IsFalse(plain.Contains("CREATE USER"));

            var owner = DatabaseTransferRunner.BuildRelinkSql(links, asDbOwner: true);
            StringAssert.Contains(owner, "ALTER ROLE db_owner ADD MEMBER [apx];");
            StringAssert.Contains(owner, "CREATE USER [apx] FOR LOGIN [apx];");
        }

        [TestMethod]
        public void DatabaseOverride_EmptyFallsBackToConnection()
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "Sales_Test", false) };
            var p = Pair(1, 2);
            p.SourceDatabase = "  ";
            Assert.AreEqual("Sales", TransferGuard.ResolveSource(p, cons).db);
            Assert.AreEqual("Sales_Test", TransferGuard.ResolveTarget(p, cons).db);
        }

        [DataTestMethod]
        [DataRow(@"\\server\share", false)]
        [DataRow(@"Backups", false)]
        [DataRow(@"D:\Back$ups", false)]
        [DataRow(@"D:\Back%TEMP%", false)]
        [DataRow(@"D:\It's", false)]
        [DataRow(@"D:\Backups", true)]
        [DataRow(@"D:/Backups/sql", true)]
        public void Validate_Folder(string folder, bool ok)
        {
            var cons = new List<Connection> { Con(1, "PROD01", "Sales", true), Con(2, "TEST01", "Sales", false) };
            var p = Pair(1, 2);
            p.SourceSsh.Folder = folder;
            Assert.AreEqual(ok, !TransferGuard.Validate(p, cons).Any(e => e.StartsWith("Source backup folder")));
        }

        [DataTestMethod]
        [DataRow("PROD01", "PROD01")]
        [DataRow(@"PROD01\INST1", "PROD01")]
        [DataRow("tcp:prod01.corp.local,1433", "prod01.corp.local")]
        [DataRow(@"PROD01\INST,1500", "PROD01")]
        [DataRow(".", "localhost")]
        [DataRow("(local)", "localhost")]
        [DataRow("", "")]
        public void HostFromServerName(string server, string expected)
        {
            Assert.AreEqual(expected, TransferGuard.HostFromServerName(server));
        }

        [TestMethod]
        public void NormalizeInstance_TreatsAliasesAsEqual()
        {
            Assert.AreEqual(TransferGuard.NormalizeInstance("prod01"), TransferGuard.NormalizeInstance("tcp:PROD01.corp.local,1433"));
            Assert.AreEqual(TransferGuard.NormalizeInstance("prod01"), TransferGuard.NormalizeInstance(@"PROD01\MSSQLSERVER"));
            Assert.AreNotEqual(TransferGuard.NormalizeInstance("prod01"), TransferGuard.NormalizeInstance(@"PROD01\TEST"));
            Assert.AreEqual(TransferGuard.NormalizeInstance("."), TransferGuard.NormalizeInstance(Environment.MachineName));
        }

        [TestMethod]
        public void ResolveSshHost_PrefersExplicitHost()
        {
            var con = Con(1, @"PROD01\INST", "Sales", true);
            Assert.AreEqual("PROD01", TransferGuard.ResolveSshHost(new SshEndpoint(), con));
            Assert.AreEqual("jump-prod", TransferGuard.ResolveSshHost(new SshEndpoint { Host = " jump-prod " }, con));
        }

        [TestMethod]
        public void ScpPaths()
        {
            Assert.AreEqual("/D:/Backups/x.bak", SshClient.ToScpPath(@"D:\Backups\x.bak"));
            Assert.AreEqual(@"D:\Backups\x.bak", SshClient.CombineWindowsPath(@"D:\Backups\", "x.bak"));
            Assert.AreEqual(@"D:\Backups\x.bak", SshClient.CombineWindowsPath("D:/Backups", "x.bak"));
        }

        [TestMethod]
        public void PowerShellCommand_IsEncodedAndShellSafe()
        {
            var cmd = SshClient.PowerShellCommand("if ($x) { \"a\" } else { '%PATH%' }");
            StringAssert.StartsWith(cmd, "powershell.exe ");
            var encoded = cmd.Substring(cmd.LastIndexOf(' ') + 1);
            Assert.IsTrue(encoded.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='));
            var decoded = System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(encoded));
            StringAssert.EndsWith(decoded, "if ($x) { \"a\" } else { '%PATH%' }");
            Assert.AreEqual("'it''s'", SshClient.PsLiteral("it's"));
        }

        [TestMethod]
        public void SqlQuoting()
        {
            Assert.AreEqual("[a]]b]", DatabaseTransferRunner.QuoteName("a]b"));
            Assert.AreEqual("N'D:\\it''s.bak'", DatabaseTransferRunner.QuoteString("D:\\it's.bak"));
            Assert.AreEqual("My_DB_1", DatabaseTransferRunner.SafeFileName("My DB$1"));
        }

        [TestMethod]
        public void BuildMoveClauses_PlacesFilesInDefaultFolders()
        {
            var files = new List<DatabaseTransferRunner.BackupFile>
            {
                new DatabaseTransferRunner.BackupFile { LogicalName = "Sales_log", Type = "L", FileId = 2 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "Sales", Type = "D", FileId = 1 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "Sales_Data2", Type = "D", FileId = 3 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "Sales FS", Type = "S", FileId = 4 },
            };

            var moves = DatabaseTransferRunner.BuildMoveClauses(files, "Sales_Test", @"F:\Data", @"G:\Log\");
            var map = moves.ToDictionary(m => m.Logical, m => m.Physical);

            Assert.AreEqual(@"F:\Data\Sales_Test.mdf", map["Sales"]);
            Assert.AreEqual(@"G:\Log\Sales_Test_log.ldf", map["Sales_log"]);
            Assert.AreEqual(@"F:\Data\Sales_Test_Sales_Data2.ndf", map["Sales_Data2"]);
            Assert.AreEqual(@"F:\Data\Sales_Test_Sales_FS", map["Sales FS"]);
        }

        [TestMethod]
        public void BuildMoveClauses_AvoidsCollisions()
        {
            var files = new List<DatabaseTransferRunner.BackupFile>
            {
                new DatabaseTransferRunner.BackupFile { LogicalName = "main", Type = "D", FileId = 1 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "a b", Type = "D", FileId = 3 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "a_b", Type = "D", FileId = 4 },
                new DatabaseTransferRunner.BackupFile { LogicalName = "log", Type = "L", FileId = 2 },
            };
            var moves = DatabaseTransferRunner.BuildMoveClauses(files, "X", @"F:\Data", @"F:\Data");
            Assert.AreEqual(moves.Count, moves.Select(m => m.Physical.ToLowerInvariant()).Distinct().Count());
            Assert.IsTrue(moves.Any(m => m.Physical == @"F:\Data\X_a_b_2.ndf"));
        }
    }
}
