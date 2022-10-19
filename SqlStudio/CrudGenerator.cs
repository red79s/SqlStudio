using System;
using System.Collections.Generic;

namespace SqlStudio
{
    public class ColumnDef
    {
        public ColumnDef()
        {
        }

        public ColumnDef(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Object Value { get; set; }

        public string GetReadableType()
        {
            if (Type == typeof(string))
                return "string";
            if (Type == typeof(long))
                return "long";
            if (Type == typeof(int))
                return "int";
            if (Type == typeof(DateTime))
                return "DateTime";
            return Type.Name;
        }
    }

    public class CrudGenerator
    {
        private string TableName { get; set; }
        private List<ColumnDef> Columns { get; set; }
 
        public CrudGenerator(string tableName, List<ColumnDef> columns)
        {
            TableName = tableName;
            Columns = columns;
        }

        private string _cmdText = "cmd.CommandText = ";
        private string _paramText = "cmd.Parameters.Add(CreateParameter(\"{0}\", null));";
        public string GenerateInsert()
        {
            string insertStmt = _cmdText + $"\"INSERT INTO {TableName}";
            string colList = "(";
            string valList = "VALUES(";
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    colList += ", ";
                    valList += ", ";
                }
                colList += Columns[i].Name;
                valList += "@" + Columns[i].Name;
            }
            var stmt = insertStmt + colList + ") " + valList + ")\";" + Environment.NewLine + GetParametersCode();
            return stmt;
        }

        public string GenerateSelect()
        {
            string selectStmt = _cmdText + "\"SELECT ";
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    selectStmt += ", ";
                }
                selectStmt += $"{Columns[i].Name}";
            }
            selectStmt += $" FROM {TableName}";
            return selectStmt + GetWhereCode() + "\"" + Environment.NewLine + GetParametersCode() + Environment.NewLine + GenerateReader();
        }

        public string GenerateReader()
        {
            string readerCode = $"using (var reader = cmd.ExecuteReader()){Environment.NewLine}{{{Environment.NewLine}";
            readerCode += "\tvar res = new List<HierarchyItem>();" + Environment.NewLine;
            readerCode += "\twhile (reader.Read())" + Environment.NewLine;
            readerCode += "\t{" + Environment.NewLine;
            readerCode += "\t\tvar item = new HierarchyItem();" + Environment.NewLine;
            for(int i = 0; i < Columns.Count; i++)
            {
                var col = Columns[i];
                readerCode += $"\t\titem.{col.Name} = reader.Get{col.Type.Name}({i});" + Environment.NewLine;
            }
            readerCode += "\t\tres.Add(item);" + Environment.NewLine;
            readerCode += "\t}" + Environment.NewLine;
            readerCode += "\treturn res;" + Environment.NewLine;
            readerCode += "}" + Environment.NewLine;
            return readerCode;
        }

        public string GenerateUpdate()
        {
            string updateStmt = _cmdText + $"\"UPDATE {TableName} SET ";
            
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    updateStmt += ", ";
                }
                updateStmt += $"{Columns[i].Name} = @{Columns[i].Name}";
            }
            return updateStmt + GetWhereCode() + "\";" + Environment.NewLine + GetParametersCode();
        }

        public string GenerateDelete()
        {
            string deleteStmt = _cmdText + $"\"DELETE FROM {TableName}";

            return deleteStmt + GetWhereCode() + "\";" + Environment.NewLine + GetParametersCode();
        }

        public string GenerateEntity()
        {
            string entityCode = $"public class {TableName}{Environment.NewLine}{{{Environment.NewLine}";
            foreach (var col in Columns)
            {
                entityCode += $"\tpublic {col.GetReadableType()} {col.Name} {{ get; set; }}{Environment.NewLine}";
            }
            entityCode += "}" + Environment.NewLine;
            return entityCode;
        }

        private string GetParametersCode()
        {
            string paramCode = "";
            foreach (var col in Columns)
            {
                paramCode += string.Format(_paramText, col.Name) + Environment.NewLine;
            }
            return paramCode;
        }

        private string GetWhereCode()
        {
            string whereStmt = " WHERE ";
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i > 0)
                {
                    whereStmt += " AND ";
                }
                whereStmt += $"{Columns[i].Name} = @{Columns[i].Name}";
            }

            return whereStmt;
        }
    }
}
