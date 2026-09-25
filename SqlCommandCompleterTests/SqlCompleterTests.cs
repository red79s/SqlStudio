using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlCommandCompleter;
using System.Collections.Generic;
using System.Linq;

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

            _databaseKeywordEscape.Setup(x => x.EscapeObject(It.IsAny<string>())).Returns((string s) => s);
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
            Assert.AreEqual(15, res.PossibleCompletions.Count);
            Assert.AreEqual("SELECT", res.PossibleCompletions[0]);
        }

        [TestMethod]
        public void TestGetPossibleCompletionsTableNames()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select * from ", 14);
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, res.PossibleCompletions.ToList());
        }

        [TestMethod]
        public void TestGetPossibleCompletionsTableNamesFoo()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions("select * from f", 15);
            CollectionAssert.AreEqual(new[] { "foo" }, res.PossibleCompletions.ToList());
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
            Assert.AreEqual("FROM", res.PossibleCompletions[0]);
            Assert.AreEqual("*", res.PossibleCompletions[1]);
            Assert.AreEqual("foo_col1", res.PossibleCompletions[2]);
            CollectionAssert.Contains(res.PossibleCompletions.ToList(), "bar_col2");
            CollectionAssert.Contains(res.PossibleCompletions.ToList(), "DISTINCT");
            CollectionAssert.Contains(res.PossibleCompletions.ToList(), "COUNT(");
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
            Assert.AreEqual("FROM", res.PossibleCompletions[0]);
            Assert.AreEqual("*", res.PossibleCompletions[1]);
            Assert.AreEqual("foo_col1", res.PossibleCompletions[2]);
            CollectionAssert.DoesNotContain(res.PossibleCompletions.ToList(), "bar_col1");
        }

        private List<string> Complete(string sql)
        {
            return CreateCompleter().GetPossibleCompletions(sql, sql.Length).PossibleCompletions.ToList();
        }

        [TestMethod]
        public void TestKeywordsAfterFromTable()
        {
            var res = Complete("select * from foo f ");
            CollectionAssert.Contains(res, "WHERE");
            CollectionAssert.Contains(res, "INNER JOIN");
            CollectionAssert.Contains(res, "ORDER BY");
            CollectionAssert.DoesNotContain(res, "foo");
        }

        [TestMethod]
        public void TestOnAfterJoinTable()
        {
            CollectionAssert.AreEqual(new[] { "ON" }, Complete("select * from foo f join bar b "));
        }

        [TestMethod]
        public void TestWhereColumnsHaveNoStarOrClauseKeywords()
        {
            var res = Complete("select * from foo where ");
            CollectionAssert.Contains(res, "foo_col1");
            CollectionAssert.DoesNotContain(res, "*");
            CollectionAssert.DoesNotContain(res, "ORDER BY");
            CollectionAssert.DoesNotContain(res, "bar_col1");
        }

        [TestMethod]
        public void TestWhereOperatorsAfterColumn()
        {
            var res = Complete("select * from foo where foo_col1 ");
            CollectionAssert.Contains(res, "=");
            CollectionAssert.Contains(res, "LIKE");
            CollectionAssert.Contains(res, "IS NULL");
            CollectionAssert.Contains(res, "ORDER BY");
            CollectionAssert.DoesNotContain(res, "foo_col1");
        }

        [TestMethod]
        public void TestWhereOperatorPrefix()
        {
            CollectionAssert.AreEqual(new[] { "IN (", "IS NULL", "IS NOT NULL" }, Complete("select * from foo where foo_col1 i"));
        }

        [TestMethod]
        public void TestNullAfterIs()
        {
            CollectionAssert.AreEqual(new[] { "NULL", "NOT NULL" }, Complete("select * from foo where foo_col1 is "));
        }

        [TestMethod]
        public void TestColumnsAfterEqualsWithoutSpace()
        {
            CollectionAssert.AreEqual(new[] { "b.bar_col1", "b.bar_col2" }, Complete("select * from foo a, bar b where a.foo_col1=b."));
        }

        [TestMethod]
        public void TestColumnsAfterAnd()
        {
            var res = Complete("select * from foo where foo_col1 = 1 and ");
            CollectionAssert.Contains(res, "foo_col2");
            CollectionAssert.DoesNotContain(res, "AND");
        }

        [TestMethod]
        public void TestOrderByColumns()
        {
            CollectionAssert.AreEqual(new[] { "foo_col1", "foo_col2" }, Complete("select * from foo order by "));
        }

        [TestMethod]
        public void TestOrderByDirectionAfterColumn()
        {
            CollectionAssert.AreEqual(new[] { "ASC", "DESC" }, Complete("select * from foo order by foo_col1 "));
        }

        [TestMethod]
        public void TestOrderByColumnsAfterComma()
        {
            CollectionAssert.AreEqual(new[] { "f.foo_col1", "f.foo_col2" }, Complete("select * from foo f order by f.foo_col1, f."));
        }

        [TestMethod]
        public void TestGroupByColumns()
        {
            CollectionAssert.AreEqual(new[] { "foo_col1", "foo_col2" }, Complete("select * from foo group by "));
        }

        [TestMethod]
        public void TestHavingAfterGroupByColumn()
        {
            CollectionAssert.AreEqual(new[] { "HAVING" }, Complete("select foo_col1 from foo group by foo_col1 h"));
        }

        [TestMethod]
        public void TestMultiLineStatement()
        {
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, Complete("select *\r\nfrom "));
            CollectionAssert.AreEqual(new[] { "f.foo_col1", "f.foo_col2" }, Complete("select *\r\nfrom foo f\r\nwhere f."));
        }

        [TestMethod]
        public void TestTabSeparatedStatement()
        {
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, Complete("select *\tfrom\t"));
        }

        [TestMethod]
        public void TestSelectAfterParenthesis()
        {
            var res = Complete("select count(");
            CollectionAssert.Contains(res, "*");
            CollectionAssert.Contains(res, "foo_col1");
        }

        [TestMethod]
        public void TestSelectFunctions()
        {
            CollectionAssert.AreEqual(new[] { "COUNT(" }, Complete("select cou"));
        }

        [TestMethod]
        public void TestInsertIntoTables()
        {
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, Complete("insert into "));
        }

        [TestMethod]
        public void TestInsertColumnList()
        {
            CollectionAssert.AreEqual(new[] { "bar_col1", "bar_col2" }, Complete("insert into bar ("));
            CollectionAssert.AreEqual(new[] { "bar_col1", "bar_col2" }, Complete("insert into bar (bar_col1, "));
        }

        [TestMethod]
        public void TestInsertAfterColumnList()
        {
            CollectionAssert.AreEqual(new[] { "VALUES", "SELECT" }, Complete("insert into bar (bar_col1) "));
        }

        [TestMethod]
        public void TestDeleteFrom()
        {
            CollectionAssert.AreEqual(new[] { "FROM" }, Complete("delete "));
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, Complete("delete from "));
            CollectionAssert.AreEqual(new[] { "WHERE" }, Complete("delete from foo "));
        }

        [TestMethod]
        public void TestUpdateSet()
        {
            CollectionAssert.AreEqual(new[] { "SET" }, Complete("update foo "));
            CollectionAssert.AreEqual(new[] { "foo_col1", "foo_col2" }, Complete("update foo set "));
            CollectionAssert.AreEqual(new[] { "WHERE" }, Complete("update foo set foo_col1 = 1 "));
        }

        [TestMethod]
        public void TestTruncateTable()
        {
            CollectionAssert.AreEqual(new[] { "TABLE" }, Complete("truncate "));
            CollectionAssert.AreEqual(new[] { "foo", "bar" }, Complete("truncate table "));
        }

        [TestMethod]
        public void TestTableAliasWithAs()
        {
            CollectionAssert.AreEqual(new[] { "f.foo_col1", "f.foo_col2" }, Complete("select * from foo as f where f."));
        }

        [TestMethod]
        public void TestCursorAtStartBeforeSeparatorDoesNotThrow()
        {
            var comp = CreateCompleter();
            var res = comp.GetPossibleCompletions(", x", 0);
            Assert.AreEqual(15, res.PossibleCompletions.Count);
        }

        private SqlCompleter CreateCompleterWithReservedColumn()
        {
            var schema = new Mock<IDatabaseSchemaInfo>();
            schema.Setup(x => x.Tables).Returns(new List<TableInfo>
            {
                new TableInfo { TableName = "foo", Columns = new List<ColumnInfo> { new ColumnInfo { ColumnName = "key" }, new ColumnInfo { ColumnName = "foo_col1" } } }
            });
            var escape = new Mock<IDatabaseKeywordEscape>();
            escape.Setup(x => x.EscapeObject(It.IsAny<string>())).Returns((string s) => s == "key" ? "[key]" : s);
            return new SqlCompleter(_logger.Object, schema.Object, escape.Object);
        }

        [TestMethod]
        public void TestReservedColumnEscapedAfterAlias()
        {
            var comp = CreateCompleterWithReservedColumn();
            var sql = "select * from foo f where f.";
            var res = comp.GetPossibleCompletions(sql, sql.Length).PossibleCompletions.ToList();
            CollectionAssert.AreEqual(new[] { "f.[key]", "f.foo_col1" }, res);
        }

        [TestMethod]
        public void TestReservedColumnMatchedWithoutTypingQuotes()
        {
            var comp = CreateCompleterWithReservedColumn();
            var sql = "select * from foo f where f.k";
            CollectionAssert.AreEqual(new[] { "f.[key]" }, comp.GetPossibleCompletions(sql, sql.Length).PossibleCompletions.ToList());

            sql = "select * from foo where k";
            CollectionAssert.AreEqual(new[] { "[key]" }, comp.GetPossibleCompletions(sql, sql.Length).PossibleCompletions.ToList());
        }
    }
}
