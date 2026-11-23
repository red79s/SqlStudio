using Common;
using Common.Model;
using CommandPrompt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlExecute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlStudioTests
{
    /// <summary>Runs batches against SQL Server LocalDB; inconclusive when LocalDB is not installed.</summary>
    [TestClass]
    [TestCategory("Integration")]
    public class SqlExecuterBatchTests
    {
        private SqlExecuter _executer;

        [TestInitialize]
        public void Connect()
        {
            _executer = new SqlExecuter(Mock.Of<ILogger>(), Mock.Of<IDatabaseKeywordEscape>());
            _executer.Connect(@"(localdb)\MSSQLLocalDB", "tempdb", null, null, SqlExecuter.DatabaseProvider.SQLSERVER);
            if (!_executer.IsConnected)
                Assert.Inconclusive("SQL Server LocalDB is not available.");
        }

        [TestCleanup]
        public void Disconnect()
        {
            _executer?.Connection?.Close();
        }

        private (bool Ok, List<SqlResult> Results, List<string> Messages) Run(string script)
        {
            var results = new List<SqlResult>();
            var messages = new List<string>();
            bool ok = true;
            foreach (var batch in SqlBatchSplitter.Split(script))
            {
                for (int i = 0; i < batch.RepeatCount && ok; i++)
                    ok = _executer.ExecuteBatch(batch.Text, batch.StartLine, results, messages.Add);
                if (!ok)
                    break;
            }
            return (ok, results, messages);
        }

        [TestMethod]
        public void VariablesTempTablesIfBlocksAndPrintWorkInOneBatch()
        {
            var (ok, results, messages) = Run(@"
SET NOCOUNT ON;
DECLARE @Commit bit = 0;   -- preview
IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T;
CREATE TABLE #T (Id int, Name nvarchar(20));
INSERT INTO #T VALUES (1, N'a;b'), (2, N'c');

SELECT Id, Name FROM #T ORDER BY Id;
SELECT COUNT(*) FROM #T;

IF @Commit = 1
BEGIN
    DELETE FROM #T;
    PRINT CONCAT('Deleted: ', @@ROWCOUNT);
END
ELSE
BEGIN
    PRINT 'Preview only - nothing was changed.';
END");

            Assert.IsTrue(ok, string.Join(Environment.NewLine, messages));
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(2, results[0].DataTable.Rows.Count);
            Assert.AreEqual("a;b", results[0].DataTable.Rows[0]["Name"]);
            Assert.AreEqual("(No column name)", results[1].DataTable.Columns[0].ColumnName);
            Assert.AreEqual(2, results[1].DataTable.Rows[0][0]);
            CollectionAssert.AreEqual(new[] { "Preview only - nothing was changed." }, messages);
        }

        [TestMethod]
        public void RowCountsAndRepeatedColumnNames()
        {
            var (ok, results, messages) = Run("CREATE TABLE #R (Id int)\nGO\nINSERT INTO #R VALUES (1), (2)\nGO 2\nSELECT a.Id, b.Id FROM #R a JOIN #R b ON a.Id = b.Id");

            Assert.IsTrue(ok, string.Join(Environment.NewLine, messages));
            CollectionAssert.AreEqual(new[] { "Id", "Id1" }, results[0].DataTable.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName).ToList());
            Assert.AreEqual(2, messages.Count(m => m == "(2 rows affected)"));
            Assert.AreEqual(8, results[0].DataTable.Rows.Count); // 4 rows, each Id twice, joined with itself
        }

        [TestMethod]
        public void ErrorReportsScriptLineAndKeepsEarlierResults()
        {
            var (ok, results, messages) = Run("SELECT 1 AS A\nGO\nSELECT 2 AS B\n\nSELECT * FROM dbo.TableThatDoesNotExist");

            Assert.IsFalse(ok);
            Assert.AreEqual(2, results.Count); // the missing table is only resolved when its statement runs
            var error = messages.Single(m => m.StartsWith("Msg 208"));
            StringAssert.Contains(error, "Line 5");   // line 5 of the script, not line 3 of the batch
            StringAssert.Contains(error, "TableThatDoesNotExist");
        }

        [TestMethod]
        public void RuntimeErrorDoesNotHideLaterResultSets()
        {
            // Without XACT_ABORT a key violation ends only the statement; SSMS still shows the result after it.
            var (ok, results, messages) = Run("CREATE TABLE #P (Id int PRIMARY KEY);\nINSERT #P VALUES (1);\nINSERT #P VALUES (1);\nSELECT 3 AS C;");

            Assert.IsFalse(ok);
            Assert.IsTrue(messages.Any(m => m.StartsWith("Msg 2627") && m.Contains("Line 3")), string.Join(Environment.NewLine, messages));
            Assert.AreEqual("C", results.Last().DataTable.Columns[0].ColumnName);
        }

        [TestMethod]
        public void ReportsOpenTransaction()
        {
            var (ok, _, _) = Run("BEGIN TRANSACTION");
            Assert.IsTrue(ok);
            Assert.AreEqual(1, _executer.GetOpenTransactionCount());
            Run("ROLLBACK");
            Assert.AreEqual(0, _executer.GetOpenTransactionCount());
        }
    }
}
