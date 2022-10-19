using System;
using System.Collections.Generic;
using System.Text;

using CodeGeneratorLib;

namespace EntityGenerator
{
    public class GenerateBaseEntity
    {
        public GenerateBaseEntity()
        {
        }

        public string Generate(string path, string nameSpace, string className, string baseClass, TableDef tableDef)
        {
            CodeGeneratorLib.EntityBaseGenerator entityBaseGenerator = new CodeGeneratorLib.EntityBaseGenerator();
            entityBaseGenerator.AddClassName(className);
            entityBaseGenerator.AddBaseClassName(baseClass);
            entityBaseGenerator.AddCodePath(path);
            entityBaseGenerator.AddNamespace(nameSpace);
            entityBaseGenerator.AddTableName(tableDef.TableName);

            foreach (ColumnDef colDef in tableDef.Columns)
            {
                if (!colDef.IsVirtual)
                    entityBaseGenerator.AddColumn(colDef.ColumnName, colDef.DataType, tableDef.GetCSName(colDef.ColumnName), colDef.IsKey, colDef.DataLength);
            }

            string result = entityBaseGenerator.Generate();

            return result;
        }
    }
}
