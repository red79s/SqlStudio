using System.Collections.Generic;
using Common;
using Common.Model;
using LlmService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LlmServiceTests
{
    [TestClass]
    public class SchemaPromptBuilderTests
    {
        private static Mock<IDatabaseSchemaInfo> CreateSchema()
        {
            var schema = new Mock<IDatabaseSchemaInfo>();
            schema.Setup(x => x.DatabaseName).Returns("MyDb");
            schema.Setup(x => x.Tables).Returns(new List<TableInfo>
            {
                new TableInfo
                {
                    TableName = "foo",
                    Columns = new List<ColumnInfo>
                    {
                        new ColumnInfo { ColumnName = "foo_id", ColumnType = "int", IsPrimaryKey = true, IsNullable = false },
                        new ColumnInfo { ColumnName = "foo_name", ColumnType = "varchar", IsNullable = true }
                    }
                },
                new TableInfo
                {
                    TableName = "bar",
                    Columns = new List<ColumnInfo>
                    {
                        new ColumnInfo { ColumnName = "bar_id", ColumnType = "int", IsPrimaryKey = true, IsNullable = false },
                        new ColumnInfo { ColumnName = "foo_id", ColumnType = "int", IsNullable = false }
                    }
                }
            });
            schema.Setup(x => x.ForeignKeys).Returns(new List<ForeignKeyInfo>
            {
                new ForeignKeyInfo
                {
                    TableName = "bar", ColumnName = "foo_id",
                    ForeignTableName = "foo", ForeignColumnName = "foo_id"
                }
            });
            return schema;
        }

        [TestMethod]
        public void Build_IncludesTablesColumnsAndForeignKeys()
        {
            var result = SchemaPromptBuilder.Build(CreateSchema().Object);

            StringAssert.Contains(result, "MyDb");
            StringAssert.Contains(result, "foo");
            StringAssert.Contains(result, "foo_name");
            StringAssert.Contains(result, "bar");
            StringAssert.Contains(result, "PRIMARY KEY");
            StringAssert.Contains(result, "bar.foo_id -> foo.foo_id");
        }

        [TestMethod]
        public void Build_NullSchema_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, SchemaPromptBuilder.Build(null));
        }

        [TestMethod]
        public void Build_NoTables_ReturnsEmpty()
        {
            var schema = new Mock<IDatabaseSchemaInfo>();
            schema.Setup(x => x.Tables).Returns(new List<TableInfo>());
            Assert.AreEqual(string.Empty, SchemaPromptBuilder.Build(schema.Object));
        }
    }
}
