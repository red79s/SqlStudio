using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorLib
{
    public class DBRowBaseGenerator
    {
        private string _templateFile = null;
        private CodeGeneratorInput _input = null;

        public DBRowBaseGenerator(string templateFile)
        {
            if (templateFile == null || templateFile == "")
                this._templateFile = "DBRowBaseTemplate.txt";
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

        public void AddColumn(string name, string type, bool keyColumn)
        {
            ArrayInst columns = _input.AddArrayInst("all_columns");
            columns.AddVariable("column_name", name);
            columns.AddVariable("column_type", DBTypeConverter.GetDotNetType(type));

            if (keyColumn)
            {
                ArrayInst keyCol = _input.AddArrayInst("key_columns");
                keyCol.AddVariable("key_column", name);
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
