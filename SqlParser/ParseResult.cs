using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;

namespace SqlParser
{
    public class ParseResult
    {
        public List<ParsedColumnInfo> ParsedColumns { get; set; }
        public List<ParsedTableInfo> ParsedTableInfo { get; set; }
        public TSqlFragment Tree { get; set; }
    }
}
