using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;
using System.IO;

namespace SqlParser
{
    public class Parser
    {
        public ParseResult Parse(string command)
        {
            using (var rdr = new StringReader(command))
            {
                IList<ParseError> errors = null;
                var parser = new TSql150Parser(true, SqlEngineType.All);
                var tree = parser.Parse(rdr, out errors);

                var tableFinder = new TableFinder();
                tree.Accept(tableFinder);

                var columnFinder = new ColumnFinder();
                tree.Accept(columnFinder);

                return new ParseResult { ParsedTableInfo = tableFinder.Tables, ParsedColumns = columnFinder.Columns, Tree = tree };
            }
        }
    }
}
