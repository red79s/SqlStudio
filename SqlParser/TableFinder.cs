using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;

namespace SqlParser
{
    public class TableFinder : TSqlFragmentVisitor
    {
        public List<ParsedTableInfo> Tables { get; set; } = new List<ParsedTableInfo>();
        public override void Visit(NamedTableReference tableRef)
        {
            var table = new ParsedTableInfo { TableName = tableRef.SchemaObject.BaseIdentifier.Value };
            if (tableRef?.Alias?.Value != null)
            {
                table.Alias = tableRef.Alias.Value;
            }
            Tables.Add(table);

            base.Visit(tableRef);
        }

    }
}
