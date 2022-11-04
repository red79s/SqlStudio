using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlStudio
{
    public class CommandParser
    {
        private char _seperator = ';';

        public CommandParser(char seperator = ';')
        {
            _seperator = seperator;
        }

        public List<string> GetCommands(string commandLine)
        {
            bool inComment = false;
            char commentChar = ' ';

            List<string> ret = new List<string>();
            string currentCommand = "";
            foreach (var c in commandLine)
            {
                if (c == '\'' || c == '"')
                {
                    if (!inComment)
                    {
                        inComment = true;
                        commentChar = c;
                    }
                    else
                    {
                        if (currentCommand[currentCommand.Length - 1] != '\\' && commentChar == c)
                        {
                            inComment = false;
                        }
                    }
                }

                if (c == _seperator && !inComment)
                {
                    currentCommand = currentCommand.Trim();
                    if (currentCommand.Length > 0)
                    {
                        ret.Add(currentCommand);
                    }
                    currentCommand = "";
                }
                else
                {
                    currentCommand += c;
                }
            }

            currentCommand = currentCommand.Trim();
            if (currentCommand.Length > 0)
            {
                ret.Add(currentCommand);
            }

            return ret;
        }

        public static int GetLastCmdSeperator(string cmd, char sep, int index)
        {
            bool bInComment = false;
            int iStart = index;
            if (iStart >= cmd.Length)
                iStart = cmd.Length - 1;

            for (int i = iStart - 1; i >= 0; i--)
            {
                if ((cmd[i] == '\'' || cmd[i] == '"') && (i > 0 || (i > 0 && cmd[i - 1] != '\\')))
                    bInComment = !bInComment;

                if (!bInComment && cmd[i] == sep)
                    return i;
            }
            return 0;
        }
    }
}
