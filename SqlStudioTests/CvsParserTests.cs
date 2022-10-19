using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlStudio.CvsImport;
using System;
using System.Collections.Generic;

namespace SqlStudioTests
{
    [TestClass]
    public class CvsParserTests
    {
        [TestMethod]
        public void TestParseRows()
        {
            var parser = new CvsParser(_testInput);
            var rows = new List<string>();
            string currentRow = "";
            while (!parser.Eof)
            {
                currentRow += parser.GetCurrent();
                parser.MoveNext();
                if (parser.EndOfRow)
                {
                    rows.Add(currentRow);
                    currentRow = "";
                }
            }

            Assert.AreEqual("col1, col2", rows[0]);
            Assert.AreEqual("data3,data4", rows[2]);
        }

        [TestMethod]
        public void TestParseColumns()
        {
            var rows = ParseData(_testInput);

            Assert.AreEqual("col1", rows[0][0]);
            Assert.AreEqual("data2", rows[1][1]);
            Assert.AreEqual("data4", rows[2][1]);
        }

        [TestMethod]
        public void TestParseWithQuote()
        {
            var parser = new CvsParser(_testInputWithQuote);
            var rows = new List<string>();
            string currentRow = "";
            while (!parser.Eof)
            {
                currentRow += parser.GetCurrent();
                parser.MoveNext();
                if (parser.EndOfRow)
                {
                    rows.Add(currentRow);
                    currentRow = "";
                }
            }

            Assert.AreEqual("col1, col2", rows[0]);
            Assert.AreEqual("data1,data2", rows[1]);
            Assert.AreEqual("data3,data4", rows[2]);
        }

        [TestMethod]
        public void TestParseColumnsWithQuote()
        {
            var rows = ParseData(_testInputWithQuote);

            Assert.AreEqual("col1", rows[0][0]);
            Assert.AreEqual("data2", rows[1][1]);
            Assert.AreEqual("data4", rows[2][1]);
        }

        [TestMethod]
        public void TestParseWithQuoteAndComma()
        {
            var parser = new CvsParser(_testInputWithQuoteAndComma);
            var rows = new List<string>();
            string currentRow = "";
            while (!parser.Eof)
            {
                currentRow += parser.GetCurrent();
                parser.MoveNext();
                if (parser.EndOfRow)
                {
                    rows.Add(currentRow);
                    currentRow = "";
                }
            }

            Assert.AreEqual("col1, col2", rows[0]);
            Assert.AreEqual("data1,data2, is good", rows[1]);
            Assert.AreEqual("data3,data4", rows[2]);
        }

        [TestMethod]
        public void TestParseColumnsWithQuoteAndComma()
        {
            var rows = ParseData(_testInputWithQuoteAndComma);

            Assert.AreEqual("col1", rows[0][0]);
            Assert.AreEqual("data2, is good", rows[1][1]);
            Assert.AreEqual("data4", rows[2][1]);
        }

        [TestMethod]
        public void TestParseColumnsWithQuoteAndCommaAndNewLine()
        {
            var rows = ParseData(_testInputWithNewLineInsideQuote);

            Assert.AreEqual("col1", rows[0][0]);
            Assert.AreEqual("data2, is good\r\nnew line comment", rows[1][1]);
            Assert.AreEqual("data4", rows[2][1]);
        }

        [TestMethod]
        public void TestParseWithEmptyCols()
        {
            var rows = ParseData(_testInputWithEmptyCol);

            Assert.AreEqual("col1", rows[0][0]);
            Assert.AreEqual("", rows[0][1]);
            Assert.AreEqual(" col2", rows[0][2]);
            Assert.AreEqual("", rows[1][1]);
            Assert.AreEqual("data4", rows[2][1]);
            Assert.AreEqual("", rows[2][2]);
        }

        private List<List<string>> ParseData(string input)
        {
            var parser = new CvsParser(input);
            var rows = new List<List<string>>();
            var currentRow = new List<string>();
            var currentCol = "";
            while (!parser.Eof)
            {
                if (parser.IsSeperator)
                {
                    currentRow.Add(currentCol);
                    currentCol = "";
                    parser.MoveNext();
                }
                else
                {
                    currentCol += parser.GetCurrent();
                    parser.MoveNext();
                }

                if (parser.EndOfRow)
                {
                    currentRow.Add(currentCol);
                    currentCol = "";
                    rows.Add(currentRow);
                    currentRow = new List<string>();
                }
            }

            return rows;
        }

        private string _testInput = "col1, col2" + Environment.NewLine +
            "data1,data2" + Environment.NewLine +
            "data3,data4";
        
        private string _testInputWithEmptyCol = "col1,, col2" + Environment.NewLine +
            "data1,,data2" + Environment.NewLine +
            "data3,data4,";

        private string _testInputWithQuote = "col1, col2" + Environment.NewLine +
            "data1,\"data2\"" + Environment.NewLine +
            "data3,data4";

        private string _testInputWithQuoteAndComma = "col1, col2" + Environment.NewLine +
            "data1,\"data2, is good\"" + Environment.NewLine +
            "data3,data4";

        private string _testInputWithNewLineInsideQuote = "col1, col2" + Environment.NewLine +
            "data1,\"data2, is good" + Environment.NewLine +
            "new line comment\"" + Environment.NewLine +
            "data3,data4";
    }
}
