using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlExecute;
using SqlStudio;

namespace SqlStudioTests
{
    [TestClass]
    public class DatabaseKeywordEscapeManagerTests
    {
        private static string Escape(SqlExecuter.DatabaseProvider provider, string value)
        {
            var manager = new DatabaseKeywordEscapeManager { ProviderSource = () => provider };
            return manager.EscapeObject(value);
        }

        [TestMethod]
        public void TestReservedWordQuotedPerProvider()
        {
            Assert.AreEqual("[user]", Escape(SqlExecuter.DatabaseProvider.SQLSERVER, "user"));
            Assert.AreEqual("[user]", Escape(SqlExecuter.DatabaseProvider.SQLSERVERCE, "user"));
            Assert.AreEqual("[user]", Escape(SqlExecuter.DatabaseProvider.SQLITE, "user"));
            Assert.AreEqual("[user]", Escape(SqlExecuter.DatabaseProvider.ODBC, "user"));
            Assert.AreEqual("\"user\"", Escape(SqlExecuter.DatabaseProvider.POSTGRESQL, "user"));
            Assert.AreEqual("\"USER\"", Escape(SqlExecuter.DatabaseProvider.ORACLE, "USER"));
            Assert.AreEqual("`user`", Escape(SqlExecuter.DatabaseProvider.MySql, "user"));
        }

        [TestMethod]
        public void TestReservedWordIsCaseInsensitive()
        {
            Assert.AreEqual("[Order]", Escape(SqlExecuter.DatabaseProvider.SQLSERVER, "Order"));
        }

        [TestMethod]
        public void TestNonReservedWordNotQuoted()
        {
            Assert.AreEqual("customer", Escape(SqlExecuter.DatabaseProvider.POSTGRESQL, "customer"));
            Assert.AreEqual("name", Escape(SqlExecuter.DatabaseProvider.SQLSERVER, "name"));
        }

        [TestMethod]
        public void TestNullValueAndNoProvider()
        {
            var manager = new DatabaseKeywordEscapeManager();
            Assert.IsNull(manager.EscapeObject(null));
            Assert.AreEqual("[key]", manager.EscapeObject("key"));
        }
    }
}
