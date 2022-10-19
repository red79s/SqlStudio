using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlStudio.ScriptExecuter;
using System;
using System.IO;
using System.Reflection;

namespace SqlStudioTests
{
    [TestClass]
    public class ScriptReaderMsSqlFormatTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SqlStudioTests.MsSqlFormatTestFile.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                var reader = new ScriptReaderMsSqlFormat();
                reader.SetStream(stream);

                var stmt1 = reader.NextStatement();
                var stmt2 = reader.NextStatement();
                var stmt3 = reader.NextStatement();
                var stmt4 = reader.NextStatement();
                var stmt5 = reader.NextStatement();
                var stmt6 = reader.NextStatement();
                var stmt7 = reader.NextStatement();
                var stmt8 = reader.NextStatement();
                var stmt9 = reader.NextStatement();
                var stmt10 = reader.NextStatement();
            }
        }
    }
}
