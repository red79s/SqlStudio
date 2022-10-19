using SqlStudio.SqlParser;
using System.Collections.Generic;

namespace SqlStudio.CommandCompletion
{
    class CommandCompletion
    {
        SqlParser.SqlParser _parser = null;
        string[] _startCommands = new string[] { "SELECT", "UPDATE", "DELETE", "INSERT", "ls" };

        public CommandCompletion()
        {
            _parser = new SqlParser.SqlParser();    
        }

        public List<string> GetCompletions(string cmd, int index)
        {
            List<string> ret = new List<string>();

            _parser.Parse(cmd);
            if (_parser.Query.ElementType == SqlElementType.SELECT)
            {
            }
            else if (_parser.Query.ElementType == SqlElementType.UPDATE)
            {
            }
            else
            {

            }
            return ret;
        }
    }
}
