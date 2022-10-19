using System;
using System.Collections.Generic;
using System.Text;

using CodeGeneratorLib;

namespace EntityGenerator
{
    public class GenerateEntity
    {
        public GenerateEntity()
        {
        }

        public string Generate(string path, string nameSpace, string className, string baseClass, TableDef tableDef)
        {
            CodeGeneratorLib.EntityGenerator entityGenerator = new CodeGeneratorLib.EntityGenerator();
            entityGenerator.AddClassName(className);
            entityGenerator.AddBaseClassName(baseClass);
            entityGenerator.AddCodePath(path);
            entityGenerator.AddNamespace(nameSpace);
            entityGenerator.AddTableName(tableDef.TableName);

            foreach (ColumnDef colDef in tableDef.Columns)
            {
                if (colDef.IsVirtual)
                    entityGenerator.AddColumn(colDef.ColumnName, colDef.DataType, tableDef.GetCSName(colDef.ColumnName), colDef.IsKey, colDef.DataLength);
            }

            string result = entityGenerator.Generate();

            return result;
        }
    }
}
