using SqlExecute;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SqlStudio.CvsImport
{
    public class CsvImporter
    {
        public static SqlResult ImportFromFile(string fileName)
        {
            var importer = new CsvImporter();
            return importer.Import(fileName);
        }

        public static List<List<string>> ImportDataFromFile(string fileName)
        {
            var importer = new CsvImporter();
            return importer.ImportData(fileName);
        }

        public SqlResult Import(string fileName)
        {
            var result = new SqlResult();

            if (!File.Exists(fileName))
            {
                result.Success = false;
                result.Message = $"File {fileName} does not exist";
            }

            result = ParseFile(fileName);
            return result;
        }

        public List<List<string>> ImportData(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new System.Exception($"File {fileName} does not exist");
            }

            var rows = GetData(fileName);

            var result = VerifyData(rows);
            if (!result.Success)
            {
                throw new System.Exception("Failed to verify data");
            }

            return rows;
        }

        public SqlResult ParseFile(string fileName)
        {
            var rows = GetData(fileName);

            var result = VerifyData(rows);
            if (!result.Success)
            {
                return result;
            }

            result.DataTable = FillTable(rows);
            result.TableName = fileName;

            return result;
        }

        private DataTable FillTable(List<List<string>> rows)
        {
            var dt = new DataTable();
            for (int i = 0; i < rows[0].Count; i++)
            {
                dt.Columns.Add(rows[0][i]);
            }

            for (int i = 1; i < rows.Count; i++)
            {
                var dr = dt.NewRow();
                for (int j = 0; j < rows[i].Count; j++)
                {
                    dr[rows[0][j]] = rows[i][j];
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private SqlResult VerifyData(List<List<string>> rows)
        {
            if (rows.Count < 1)
            {
                return new SqlResult()
                {
                    Success = false,
                    Message = "No data"
                };
            }

            int columns = rows[0].Count;

            for (int i = 1; i < rows.Count; i++)
            {
                if (rows[i].Count != columns)
                {
                    return new SqlResult()
                    {
                        Success = false,
                        Message = $"Wrong number of columns in row: {i}, number of columns is: {rows[i].Count}, number of headers: {columns}"
                    };
                }
            }

            for (int i = 0; i < rows[0].Count; i++)
            {
                if (rows[0][i].Length > 0 && rows[0][i][0] == '@')
                {
                    rows[0][i] = rows[0][i].Substring(1, rows[0][i].Length - 1);
                }
            }

            return new SqlResult()
            {
                Success = true
            };
        }

        private List<List<string>> GetData(string fileName)
        {
            var input = File.ReadAllText(fileName);
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
    }
}
