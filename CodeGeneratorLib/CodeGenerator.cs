using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CodeGeneratorLib
{
    public class CodeGenerator
    {
        private string _templateText = null;
        private string _generatorOutput = null;

        public CodeGenerator(string templateText)
        {
            this._templateText = templateText;
        }

        public CodeGenerator()
        {
            
        }

        public bool LoadTemplateFile(string fileName)
        {
            this._templateText = File.ReadAllText(fileName);
            return true;
        }

        public bool SaveGeneratedToFile(string fileName, bool bOwerWrite)
        {
            if (this._generatorOutput != null)
            {
                if (bOwerWrite && File.Exists(fileName))
                    File.Delete(fileName);

                File.WriteAllText(fileName, this._generatorOutput);
            }
            return true;
        }

        public string GenerateFromTemplate(CodeGeneratorInput input, string templateText)
        {
            this._templateText = templateText;
            return this.GenerateFromTemplate(input);
        }

        public string GenerateFromTemplate(CodeGeneratorInput input)
        {
            if (this._templateText == null || input == null)
                return "";
            if (this._templateText.Length < 2)
                return this._templateText;

            StringBuilder sb = new StringBuilder(this._templateText.Length + (int)((float)this._templateText.Length * 0.2f));

            this.GenerateSection(this._templateText, null, input, input.GetRoot(), 0, ref sb);

            this._generatorOutput = sb.ToString();
            return this._generatorOutput;
        }

        private void GenerateSection(string templateStr, string arraySeperatorString, CodeGeneratorInput input, List<ArrayInst> array, int iOuterIndex, ref StringBuilder sb)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Count; i++)
            {
                ArrayInst arrayInst = array[i];

                if ((i > 0 || arrayInst.WriteSeperatorOnFirstElement) && arraySeperatorString != null)
                {
                    sb.Append(arraySeperatorString);
                }

                int iLastEnd = 0;
                while (true)
                {
                    int begin = 0;
                    int end = 0;
                    string varName = "";
                    bool bArray = false;

                    if (!this.GetNextVariable(templateStr, iLastEnd, ref varName, ref begin, ref end, ref bArray))
                    {
                        string strAppend = templateStr.Substring(iLastEnd, templateStr.Length - iLastEnd);
                        sb.Append(strAppend);
                        break;
                    }
                    if (bArray)
                    {
                        string strAppend = templateStr.Substring(iLastEnd, begin - iLastEnd);
                        sb.Append(strAppend);

                        string strArrSeperator = null;
                        if (templateStr[end] == '[')
                        {
                            int iSepEnd = templateStr.IndexOf(']', end + 1);
                            if (iSepEnd < end + 1)
                                throw new Exception(string.Format("Mismatched array seperator: {0} at Line: {1}", varName, this.GetLineNumber(iOuterIndex + begin)));
                            strArrSeperator = templateStr.Substring(end + 1, iSepEnd - (end + 1));
                            end += strArrSeperator.Length + 2;
                        }
                        int iArrEndIndex = templateStr.IndexOf("<%" + varName + "%>", end);
                        if (iArrEndIndex < 0)
                            throw new Exception(string.Format("Mismatched array: {0} at Line: {1}", varName, this.GetLineNumber(iOuterIndex + begin)));
                        

                        string strSubstring = templateStr.Substring(end, iArrEndIndex - end);

                        List<ArrayInst> subArrays = arrayInst.GetArray(varName);
                        this.GenerateSection(strSubstring, strArrSeperator, input, arrayInst.GetArray(varName), iOuterIndex + end, ref sb);
                        iLastEnd = iArrEndIndex + varName.Length + 4;
                    }
                    else
                    {
                        sb.Append(templateStr.Substring(iLastEnd, begin - iLastEnd));
                        if (this.IsVarBuiltin(varName))
                        {
                            string strBuiltin = this.GetBuiltin(varName);
                            sb.Append(strBuiltin);
                        }
                        else
                        {
                            string strValue = arrayInst.GetVariable(varName);
                            if (strValue == null)
                                strValue = input.GetGlobal(varName);
                            if (strValue == null)
                                strValue = "";
                            sb.Append(strValue);
                        }
                        iLastEnd = end;
                    }
                }
            }
        }

        private bool GetNextVariable(string input, int startingIndex, ref string variableName, ref int begin, ref int end, ref bool bArray)
        {
            Regex regex = new Regex("%[a-zA-Z0-9_]{1,99}%");
            Match m = regex.Match(input, startingIndex);
            if (m.Success)
            {
                begin = m.Index;
                end = m.Index + m.Length;
                variableName = m.Value.Substring(1, m.Value.Length - 2);
                bArray = false;
                if (begin > 0 && end < input.Length - 1)
                {
                    if (input[begin - 1] == '<' && input[end] == '>')
                    {
                        begin--;
                        end++;
                        bArray = true;
                    }
                }

                return true;
            }

            return false;
        }

        private bool IsVarBuiltin(string var)
        {
            switch (var)
            {
                case "TIMESTAMP": return true;
                case "USERNAME": return true;
                default: return false;
            }
        }

        private string GetBuiltin(string var)
        {
            switch (var)
            {
                case "TIMESTAMP": return DateTime.Now.ToShortDateString();
                case "USERNAME": return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                default: return "";
            }
        }

        private int GetLineNumber(int index)
        {
            int startIndex = 0;
            int line = 0;
            while ((startIndex = this._templateText.IndexOf(Environment.NewLine, startIndex + 1)) > 0)
            {
                line++;
                if (startIndex > index)
                    break;
            }
            return line;
        }
    }
}
