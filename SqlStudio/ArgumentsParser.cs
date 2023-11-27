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
            public string Flag;
            public string Value;
            public CmdPairs(string flag, string value)
            {
                Flag = flag;
                Value = value;
            }
        }
        List<CmdPairs> _cmdPairs = null;

        public ArgumentsParser(string cmdLine)
        {
            CommandLine(cmdLine);
        }

        public void CommandLine(string cmdLine)
        {
            string cleanCmdLine = cmdLine.Replace("\r\n", "\n").Replace("\n", " ");
            Parse(cleanCmdLine);
        }

        private void Parse(string cmdLine)
        {
            _cmdPairs = new List<CmdPairs>();
            _flags = new List<char>();
            _noFlagArgs = new List<string>();

            List<string> stack = new List<string>();
            MatchCollection matches = Regex.Matches(cmdLine, "(\"([^\"]*)\")|('([^']*)')|([^ ]+)");
            foreach (Match m in matches)
            {
                stack.Add(m.Groups[0].Value);
            }

            if (stack.Count > 0)
            {
                _command = GetNonQuotedArg(stack[0]);
                stack.RemoveAt(0);
            }

            bool bLastArgumentFlag = false;
            CmdPairs cmdPair = new CmdPairs("", "");
            for (int i = 0; i < stack.Count; i++)
            {
                if (bLastArgumentFlag)
                {
                    cmdPair.Value = GetNonQuotedArg(stack[i]);
                    _cmdPairs.Add(cmdPair);
                    bLastArgumentFlag = false;
                }
                else if (stack[i].IndexOf("--") == 0)
                {
                    cmdPair.Flag = stack[i].Substring(2, stack[i].Length - 2);
                    bLastArgumentFlag = true;
                }
                else if (stack[i].IndexOf("-") == 0)
                {
                    for (int j = 1; j < stack[i].Length; j++)
                    {
                        _flags.Add(stack[i][j]);
                    }
                    bLastArgumentFlag = false;
                }
                else
                {
                    _noFlagArgs.Add(GetNonQuotedArg(stack[i]));
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
            return _command;
        }

        public char[] GetFlags()
        {
            return _flags.ToArray();
        }

        public bool FlagIsSet(char flag)
        {
            foreach (char c in _flags)
            {
                if (c == flag)
                    return true;
            }
            return false;
        }

        public string[] GetNonNamedArgs()
        {
            return _noFlagArgs.ToArray();
        }

        public string GetNonNamedArg(int index)
        {
            if (index < _noFlagArgs.Count && index >= 0)
                return _noFlagArgs[index];
            return null;
        }

        public int NumNonNamedArgs
        {
            get { return _noFlagArgs.Count; }
        }

        public string GetNamedArg(string flag)
        {
            foreach (CmdPairs cp in _cmdPairs)
            {
                if (cp.Flag == flag)
                    return cp.Value;
            }
            return null;
        }

        public bool NamedArgExists(string flag)
        {
            foreach (CmdPairs cp in _cmdPairs)
            {
                if (cp.Flag == flag)
                    return true;
            }
            return false;
        }
    }
}
