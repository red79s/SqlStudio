using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace CodeGeneratorLib
{
    public class EntityGenerator
    {
        private CodeGeneratorInput _input = null;

        public EntityGenerator()
        {
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

        public void AddBaseClassName(string baseClass)
        {
            this._input.AddGlobal("base_class", baseClass);
        }

        public void AddColumn(string name, string type, string fieldName, bool isKey, int dataLength)
        {
            string csType = DBTypeConverter.GetDotNetType(type);

            ArrayInst arrayInst = _input.AddArrayInst("virtual_columns");
            arrayInst.AddVariable("column_name", name);
            arrayInst.AddVariable("column_type", csType);
            arrayInst.AddVariable("field_name", fieldName);
 
            if (isKey)
            {
                ArrayInst fieldOptons = arrayInst.AddArrayInst("field_options");
                fieldOptons.AddVariable("field_option", "IsKey = true");
                fieldOptons.WriteSeperatorOnFirstElement = true;
            }
            if (csType == "string" && dataLength > 0)
            {
                ArrayInst fieldOptons = arrayInst.AddArrayInst("field_options");
                fieldOptons.AddVariable("field_option", "DataLength = " + dataLength.ToString());
                fieldOptons.WriteSeperatorOnFirstElement = true;
            }
        }

        public string Generate()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            TextReader reader = new StreamReader(assembly.GetManifestResourceStream("CodeGeneratorLib.EntityGenerator.txt"));
            string template = reader.ReadToEnd();
            reader.Dispose();

            CodeGenerator gen = new CodeGenerator(template);
            return gen.GenerateFromTemplate(this._input);
        }
    }
}

