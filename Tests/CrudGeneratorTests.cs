using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlStudio;

namespace Tests
{
    [TestClass]
    public class CrudGeneratorTests
    {
        [TestMethod]
        public void TestGenerateInsert()
        {
            var gen = new CrudGenerator("Foo", new List<ColumnDef> { new ColumnDef("Col1", typeof(int)), new ColumnDef("Col2", typeof(string)), new ColumnDef("Col3", typeof(long)) });
            var insert = gen.GenerateInsert();
            var update = gen.GenerateUpdate();
            var del = gen.GenerateDelete();
            var sel = gen.GenerateSelect();
            var entity = gen.GenerateEntity();
        }
    }
}
