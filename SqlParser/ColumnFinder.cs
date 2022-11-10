using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Collections.Generic;

namespace SqlParser
{
    public class ColumnFinder : TSqlFragmentVisitor
    {
        public List<ParsedColumnInfo> Columns { get; set; } = new List<ParsedColumnInfo>();
        public override void Visit(ColumnReferenceExpression node)
        {
            if (node.MultiPartIdentifier.Identifiers.Count == 2)
            {
                Columns.Add(new ParsedColumnInfo
                {
                    ColumnName = node.MultiPartIdentifier.Identifiers[1].Value,
                    TableAlias = node.MultiPartIdentifier.Identifiers[0].Value
                });
            }
            else if (node.MultiPartIdentifier.Identifiers.Count == 1)
            {
                Columns.Add(new ParsedColumnInfo
                {
                    ColumnName = node.MultiPartIdentifier.Identifiers[0].Value
                });
            }
            base.Visit(node);
        }
    }
}
