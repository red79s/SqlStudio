using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlStudio.EnumImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudioTests
{
    [TestClass]
    public class EfEnumImporterTests
    {
        [TestMethod]
        public void TestLoad_sD2Data()
        {
            var importer = new EfEnumImporter();
            var res = importer.Import("C:\\src\\APX\\Avatar\\ApxSystems.AvatarServerGUI\\bin\\Debug\\sD2.Data.dll");

            var articleType = res.FirstOrDefault(x => x.TableName == "Article" && x.ColumnName == "ExpirationType");
            Assert.IsNotNull(articleType);
            Assert.AreEqual("BestBeforeDays", articleType.EnumValues[1].Name);
        }
    }
}
