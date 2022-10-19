using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorLib
{
    public class DBTableBaseGenerator
    {
        private string _templateFile = null;
        private CodeGeneratorInput _input = null;

        public DBTableBaseGenerator(string templateFile)
        {
            if (templateFile == null || templateFile == "")
                this._templateFile = "DBTableBaseTemplate.txt";
            else
                this._templateFile = templateFile;
            this._input = new CodeGeneratorInput();
        }

        public void Clear()
        {
            this._input.Clear();
        }

        public void AddCodePath(string codePath)
        {
            this._input.AddGlobal("code_path", codePath);
        }

        public void AddNamespace(string nameSpace)
        {
            this._input.AddGlobal("namespace", nameSpace);
        }

        public void AddTableName(string tableName)
        {
            this._input.AddGlobal("table_name", tableName);
        }

        public void AddClassName(string className)
        {
            this._input.AddGlobal("class_name", className);
        }

        public void AddRowClassName(string className)
        {
            this._input.AddGlobal("row_class_name", className);
        }

        public void AddColumnSpecification(string colSpecification)
        {
            this._input.AddGlobal("column_specification", colSpecification);
        }

        public void AddColumn(string name, string type, bool keyColumn, bool virtualColumn, string virtualCmd)
        {
            ArrayInst insArray = _input.AddArrayInst("insert_columns");
            ArrayInst selArray = _input.AddArrayInst("select_columns");
            if (!virtualColumn)
            {
                insArray.AddVariable("insert_column", name);
                selArray.AddVariable("select_column", name);
            }
            else
            {
                selArray.AddVariable("select_column", virtualCmd + " AS '" + name + "'");
            }

            if (keyColumn)
            {
                ArrayInst keyArray = _input.AddArrayInst("key_columns");
                keyArray.AddVariable("key_column", name);
                keyArray.AddVariable("key_type", DBTypeConverter.GetDotNetType(type));
            }
            else if (!virtualColumn)
            {
                ArrayInst updArray = _input.AddArrayInst("update_columns");
                updArray.AddVariable("update_column", name);
            }
        }

        public string Generate()
        {
            CodeGenerator gen = new CodeGenerator();
            gen.LoadTemplateFile(this._templateFile);
            return gen.GenerateFromTemplate(this._input);
        }
    }
}
