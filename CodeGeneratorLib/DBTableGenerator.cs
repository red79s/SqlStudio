using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneratorLib
{
    public class DBTableGenerator
    {
        private string _templateFile = null;
        private CodeGeneratorInput _input = null;

        public DBTableGenerator(string templateFile)
        {
            if (templateFile == null || templateFile == "")
                this._templateFile = "DBTableTemplate.txt";
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

        public void AddClassName(string className)
        {
            this._input.AddGlobal("class_name", className);
        }

        public void AddBaseClassName(string className)
        {
            this._input.AddGlobal("base_class_name", className);
        }

        public string Generate()
        {
            CodeGenerator gen = new CodeGenerator();
            gen.LoadTemplateFile(this._templateFile);
            return gen.GenerateFromTemplate(this._input);
        }
    }
}
