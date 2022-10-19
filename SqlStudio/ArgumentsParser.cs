using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlStudio
{
    public class ArgumentsParser
    {
        private List<char> _flags = null;
        private List<string> _noFlagArgs = null;
        private string _command = null;
        private struct CmdPairs
        {
            public string flag;
            public string value;
            public CmdPairs(string flag, string value)
            {
                this.flag = flag;
                this.value = value;
            }
        }
        List<CmdPairs> _cmdPairs = null;

        public ArgumentsParser(string cmdLine)
        {
            this.CommandLine(cmdLine);
        }

        public void CommandLine(string cmdLine)
        {
            string cleanCmdLine = cmdLine.Replace("\r\n", "\n").Replace("\n", " ");
            this.Parse(cleanCmdLine);
        }

        private void Parse(string cmdLine)
        {
            this._cmdPairs = new List<CmdPairs>();
            this._flags = new List<char>();
            this._noFlagArgs = new List<string>();

            List<string> stack = new List<string>();
            MatchCollection matches = Regex.Matches(cmdLine, "(\"([^\"]*)\")|('([^']*)')|([^ ]+)");
            foreach (Match m in matches)
            {
                stack.Add(m.Groups[0].Value);
            }

            if (stack.Count > 0)
            {
                this._command = this.GetNonQuotedArg(stack[0]);
                stack.RemoveAt(0);
            }

            bool bLastArgumentFlag = false;
            CmdPairs cmdPair = new CmdPairs("", "");
            for (int i = 0; i < stack.Count; i++)
            {
                if (bLastArgumentFlag)
                {
                    cmdPair.value = this.GetNonQuotedArg(stack[i]);
                    this._cmdPairs.Add(cmdPair);
                    bLastArgumentFlag = false;
                }
                else if (stack[i].IndexOf("--") == 0)
                {
                    cmdPair.flag = stack[i].Substring(2, stack[i].Length - 2);
                    bLastArgumentFlag = true;
                }
                else if (stack[i].IndexOf("-") == 0)
                {
                    for (int j = 1; j < stack[i].Length; j++)
                    {
                        this._flags.Add(stack[i][j]);
                    }
                    bLastArgumentFlag = false;
                }
                else
                {
                    this._noFlagArgs.Add(this.GetNonQuotedArg(stack[i]));
                }
            }
        }

        private string GetNonQuotedArg(string arg)
        {
            if (arg == null)
                return null;
            if (arg.Length < 2)
                return arg;
            
            if (arg[0] == '"' && arg[arg.Length - 1] == '"')
                return arg.Substring(1, arg.Length - 2);
            else if (arg[0] == '\'' && arg[arg.Length - 1] == '\'')
                return arg.Substring(1, arg.Length - 2);
            
            return arg;
        }

        public string GetCommand()
        {
            return this._command;
        }

        public char[] GetFlags()
        {
            return this._flags.ToArray();
        }

        public bool FlagIsSet(char flag)
        {
            foreach (char c in this._flags)
            {
                if (c == flag)
                    return true;
            }
            return false;
        }

        public string[] GetNonNamedArgs()
        {
            return this._noFlagArgs.ToArray();
        }

        public string GetNonNamedArg(int index)
        {
            if (index < this._noFlagArgs.Count && index >= 0)
                return this._noFlagArgs[index];
            return null;
        }

        public int NumNonNamedArgs
        {
            get { return this._noFlagArgs.Count; }
        }

        public string GetNamedArg(string flag)
        {
            foreach (CmdPairs cp in this._cmdPairs)
            {
                if (cp.flag == flag)
                    return cp.value;
            }
            return null;
        }

        public bool NamedArgExists(string flag)
        {
            foreach (CmdPairs cp in this._cmdPairs)
            {
                if (cp.flag == flag)
                    return true;
            }
            return false;
        }
    }
}
