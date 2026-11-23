using CommandPrompt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SqlStudioTests
{
    [TestClass]
    public class SqlTokenizerTests
    {
        private static List<(SqlTokenType Type, string Text)> Tokenize(string line, ref SqlLexState state)
        {
            return SqlTokenizer.TokenizeLine(line, ref state)
                .Where(t => t.Type != SqlTokenType.Whitespace)
                .Select(t => (t.Type, line.Substring(t.Index, t.Length)))
                .ToList();
        }

        private static List<(SqlTokenType Type, string Text)> Tokenize(string line)
        {
            var state = SqlLexState.Normal;
            return Tokenize(line, ref state);
        }

        [TestMethod]
        public void ClassifiesTsqlConstructs()
        {
            var tokens = Tokenize("DECLARE @ProjectId bigint = 12; -- NULL for all");

            CollectionAssert.AreEqual(new[]
            {
                (SqlTokenType.Keyword, "DECLARE"),
                (SqlTokenType.Variable, "@ProjectId"),
                (SqlTokenType.DataType, "bigint"),
                (SqlTokenType.Operator, "="),
                (SqlTokenType.Number, "12"),
                (SqlTokenType.Punctuation, ";"),
                (SqlTokenType.Comment, "-- NULL for all"),
            }, tokens);
        }

        [TestMethod]
        public void FunctionOnlyWhenCalled()
        {
            var tokens = Tokenize("SELECT LEFT(m.Batch, 3) FROM a LEFT JOIN b ON 1=1");
            Assert.AreEqual(SqlTokenType.Function, tokens.First(t => t.Text == "LEFT").Type);
            Assert.AreEqual(SqlTokenType.Keyword, tokens.Last(t => t.Text == "LEFT").Type);
        }

        [TestMethod]
        public void StringsWithEscapedQuotesAndUnicodePrefix()
        {
            var tokens = Tokenize("SELECT N'it''s', 'x' + N''' (unit)'''");
            CollectionAssert.AreEqual(new[] { "N'it''s'", "'x'", "N''' (unit)'''" },
                tokens.Where(t => t.Type == SqlTokenType.String).Select(t => t.Text).ToList());
        }

        [TestMethod]
        public void TempTablesGlobalsAndBrackets()
        {
            var tokens = Tokenize("SELECT @@ROWCOUNT FROM tempdb..#Orphans x JOIN [dbo].[My Table] y");
            Assert.IsTrue(tokens.Contains((SqlTokenType.GlobalVariable, "@@ROWCOUNT")));
            Assert.IsTrue(tokens.Contains((SqlTokenType.TempTable, "#Orphans")));
            Assert.IsTrue(tokens.Contains((SqlTokenType.QuotedIdentifier, "[My Table]")));
        }

        [TestMethod]
        public void BlockCommentSpansLinesAndNests()
        {
            var state = SqlLexState.Normal;
            var l1 = Tokenize("SELECT 1 /* start /* nested */", ref state);
            Assert.AreEqual((SqlTokenType.Comment, "/* start /* nested */"), l1.Last());
            Assert.AreEqual(1, state.CommentDepth);

            var l2 = Tokenize("still comment */ SELECT", ref state);
            Assert.AreEqual((SqlTokenType.Comment, "still comment */"), l2[0]);
            Assert.AreEqual((SqlTokenType.Keyword, "SELECT"), l2[1]);
            Assert.IsTrue(state.IsNormal);
        }

        [TestMethod]
        public void StringSpansLines()
        {
            var state = SqlLexState.Normal;
            Tokenize("PRINT 'first", ref state);
            Assert.AreEqual('\'', state.OpenQuote);
            var l2 = Tokenize("second' + 'x'", ref state);
            Assert.AreEqual((SqlTokenType.String, "second'"), l2[0]);
            Assert.IsTrue(state.IsNormal);
        }

        [TestMethod]
        public void RecognisesGoLines()
        {
            Assert.IsTrue(SqlTokenizer.TryParseBatchSeparator("GO", out _, out _, out int r1));
            Assert.AreEqual(1, r1);
            Assert.IsTrue(SqlTokenizer.TryParseBatchSeparator("  go 5  -- repeat", out _, out _, out int r2));
            Assert.AreEqual(5, r2);
            Assert.IsFalse(SqlTokenizer.TryParseBatchSeparator("GOTO label", out _, out _, out _));
            Assert.IsFalse(SqlTokenizer.TryParseBatchSeparator("GO SELECT 1", out _, out _, out _));
            Assert.IsFalse(SqlTokenizer.TryParseBatchSeparator("SELECT 1 GO", out _, out _, out _));
        }
    }

    [TestClass]
    public class SqlBatchSplitterTests
    {
        [TestMethod]
        public void ScriptWithoutGoIsOneBatch()
        {
            var batches = SqlBatchSplitter.Split(CleanupScript);

            Assert.AreEqual(1, batches.Count);
            Assert.AreEqual(0, batches[0].StartLine);
            // Statements, comments and the IF/BEGIN/END blocks are kept together, line breaks included.
            Assert.AreEqual(CleanupScript.Replace("\r\n", "\n"), batches[0].Text.Replace("\r\n", "\n"));
        }

        [TestMethod]
        public void SplitsOnGoAndKeepsLineNumbers()
        {
            var batches = SqlBatchSplitter.Split("CREATE TABLE #t (a int)\nGO\n\nINSERT #t VALUES (1)\nGO 3\nSELECT * FROM #t");

            Assert.AreEqual(3, batches.Count);
            Assert.AreEqual("CREATE TABLE #t (a int)", batches[0].Text);
            Assert.AreEqual(0, batches[0].StartLine);
            Assert.AreEqual(2, batches[1].StartLine);
            Assert.AreEqual(3, batches[1].RepeatCount);
            Assert.AreEqual("SELECT * FROM #t", batches[2].Text);
            Assert.AreEqual(5, batches[2].StartLine);
        }

        [TestMethod]
        public void GoInsideCommentOrStringDoesNotSplit()
        {
            var batches = SqlBatchSplitter.Split("SELECT 1\n/*\nGO\n*/\nPRINT 'a\nGO\nb'");
            Assert.AreEqual(1, batches.Count);
        }

        [TestMethod]
        public void EmptyAndCommentOnlyBatchesAreSkipped()
        {
            var batches = SqlBatchSplitter.Split("-- header\nGO\n\nGO\nSELECT 1\nGO\n/* trailing */");
            Assert.AreEqual(1, batches.Count);
            Assert.AreEqual("SELECT 1", batches[0].Text);
        }

        internal const string CleanupScript = @"/*
    Cleanup: module components left behind by deleted production planner elements.
    Run it as it is first: @Commit = 0 only lists what would be removed and changes nothing.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @ProjectId bigint = 12;   -- NULL for all projects
DECLARE @Commit    bit    = 0;    -- 0: preview only, 1: delete

IF OBJECT_ID('tempdb..#Orphans') IS NOT NULL DROP TABLE #Orphans;

SELECT @sql = @sql + N'
INSERT INTO #InUse (ArticleId, ReferencedBy)
SELECT DISTINCT x.' + QUOTENAME(c.name) + N', N''' + t.name + N'.' + c.name + N'''
FROM x;'
FROM sys.columns c;

IF @Commit = 1
BEGIN
    BEGIN TRANSACTION;
    PRINT CONCAT('Deleted component rows: ', @@ROWCOUNT);
    COMMIT TRANSACTION;
END
ELSE
BEGIN
    PRINT 'Preview only - nothing was changed. Set @Commit = 1 to delete.';
END";
    }
}
