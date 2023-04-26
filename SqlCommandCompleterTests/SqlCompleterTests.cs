using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlCommandCompleter;
using System.Collections.Generic;

namespace SqlCommandCompleterTests
{
    [TestClass]
    public class SqlCompleterTests
    {
        Mock<ILogger> _logger = new Mock<ILogger> ();
        Mock<IDatabaseSchemaInfo> _databaseSchemaInfo = new Mock<IDatabaseSchemaInfo> ();
        Mock<IDatabaseKeywordEscape> _databaseKeywordEscape = new Mock<IDatabaseKeywordEscape> ();
        private void Setup()
        {
            _databaseSchemaInfo.Setup(x => x.DatabaseName).Returns("MyDb");
            _databaseSchemaInfo.Setup(x => x.Tables).Returns(
                new List<TableInfo>
                {
                    new TableInfo { TableName = "foo", Columns = new List<ColumnInfo> { new ColumnInfo {ColumnName = "foo_col1" }, new ColumnInfo {ColumnName = "foo_col2"} } },
                    new TableInfo { TableName = "bar", Columns = new List<ColumnInfo> { new ColumnInfo {ColumnName = "bar_col1" }, new ColumnInfo {ColumnName = "bar_col2"} } }
                });
        }

        private SqlCompleter CreateCompleter()
        {
            Setup();
            return new SqlCompleter(_logger.Object, _databaseSchemaInfo.Object, _databaseKeywordEscape.Object);
        }

        [TestMethod]
        public void TestGetSymbolEmptyString()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("", 0);
            Assert.AreEqual("", sym.Text);
            Assert.AreEqual(0, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolOneCharIndexAfter()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("a", 1);
            Assert.AreEqual("a", sym.Text);
            Assert.AreEqual(0, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolOneCharIndexBefore()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("a", 0);
            Assert.AreEqual("a", sym.Text);
            Assert.AreEqual(0, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolFourCharIndexBefore()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("abcd", 0);
            Assert.AreEqual("abcd", sym.Text);
            Assert.AreEqual(0, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolFourCharIndexAfter()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("abcd", 4);
            Assert.AreEqual("abcd", sym.Text);
            Assert.AreEqual(0, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolTwoWordsCharIndexBefore()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("one two", 4);
            Assert.AreEqual("two", sym.Text);
            Assert.AreEqual(4, sym.Index);
        }

        [TestMethod]
        public void TestGetSymbolFourCharIndexMidle()
        {
            var comp = CreateCompleter();
            var sym = comp.GetSymbol("one two", 5);
            Assert.AreEqual("two", sym.Text);
            Assert.AreEqual(4, sym.Index);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsSelect()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("S", 1);
            Assert.AreEqual(1, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsSelectLower()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("s", 1);
            Assert.AreEqual(1, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsNoText()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("", 0);
            Assert.AreEqual(5, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsTableNames()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select * from ", 14);
            Assert.AreEqual(3, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsTableNamesFoo()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select * from f", 15);
            Assert.AreEqual(1, res.PossibleCompletions.Count);
            Assert.AreEqual("foo", res.PossibleCompletions[0]);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsFrom()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select * f", 10);
            Assert.AreEqual(3, res.PossibleCompletions.Count);
            Assert.AreEqual("FROM", res.PossibleCompletions[0]);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsCol()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select ", 7);
            Assert.AreEqual(6, res.PossibleCompletions.Count);
            Assert.AreEqual("*", res.PossibleCompletions[1]);
            Assert.AreEqual("foo_col1", res.PossibleCompletions[2]);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsColWithSeachText()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select fo", 9);
            Assert.AreEqual(2, res.PossibleCompletions.Count);
            Assert.AreEqual("foo_col1", res.PossibleCompletions[0]);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsColWithTable()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select fo from bar", 9);
            Assert.AreEqual(0, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsColWithTwoTables()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select from bar a, foo b where b.", 33);
            Assert.AreEqual(2, res.PossibleCompletions.Count);
        }

        [TestMethod]
        public void TestGetSymbols()
        {
            var comp = CreateCompleter();
            var symbols = comp.GetSymbols("select * from ");
            Assert.AreEqual(7, symbols.Count);
        }

        [TestMethod]
        public void TestGetSymbols2()
        {
            var comp = CreateCompleter();
            var symbols = comp.GetSymbols("select col1,col2 ,col3 from foo a");
            Assert.AreEqual(15, symbols.Count);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsInBetweenSpaces()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("SELECT  FROM foo;", 7);
            Assert.AreEqual(4, res.PossibleCompletions.Count);
            Assert.AreEqual("FROM", res.PossibleCompletions[0]);
            Assert.AreEqual("foo_col1", res.PossibleCompletions[2]);
        }
    }
}
