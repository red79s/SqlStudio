using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityGenerator
{
    public class TableDef
    {
        private string _tableName = null;
        private List<ColumnDef> _columns = new List<ColumnDef>();

        public TableDef(string tableName)
        {
            _tableName = tableName;
        }

        public string GetCSName(string input)
        {
            if (input.Length < 1)
                return "";

            string res = "";
            string[] parts = input.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                res += Char.ToUpper(parts[i][0]) + parts[i].Substring(1, parts[i].Length - 1);
            }
            return res;
        }

        public string TableName
        {
            get { return _tableName; }
        }

        public List<ColumnDef> Columns
        {
            get { return _columns; }
        }
    }
}
