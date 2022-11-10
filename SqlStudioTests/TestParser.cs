using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlParser;

namespace SqlStudioTests
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void Test()
        {
            var parser = new Parser();
            var res = parser.Parse("select * from foo");
            Assert.AreEqual(0, res.ParsedColumns.Count);
            Assert.AreEqual("foo", res.ParsedTableInfo[0].TableName);
        }

        [TestMethod]
        public void TestJoin()
        {
            var parser = new Parser();
            var res = parser.Parse("select col1, b.col2, a.col3 from foo a , bar b");
            Assert.AreEqual(3, res.ParsedColumns.Count);
            Assert.AreEqual("foo", res.ParsedTableInfo[0].TableName);
            Assert.AreEqual("a", res.ParsedTableInfo[0].Alias);
            Assert.AreEqual("bar", res.ParsedTableInfo[1].TableName);
            Assert.AreEqual("b", res.ParsedTableInfo[1].Alias);
        }
    }
}
