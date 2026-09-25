using System;
using System.Collections.Generic;
using System.Text;

namespace SqlStudio.SyntaxHighlight
{
    class SQLSyntaxHighlight : SyntaxHighlightBase
    {
        // Single words only, the tokenizer splits on whitespace and punctuation.
        // Words that are commonly used as column names (name, date, time, text, type, value, ...) are left out.
        private static readonly HashSet<string> _keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Statements
            "select", "insert", "update", "delete", "merge", "truncate", "create", "alter", "drop",
            "exec", "execute", "declare", "print", "use", "go", "grant", "revoke", "deny",

            // Clauses
            "from", "where", "into", "values", "set", "output", "returning",
            "group", "order", "by", "having", "limit", "offset", "fetch", "next", "rows", "only", "top", "percent",
            "union", "intersect", "except", "all", "distinct", "with", "as",

            // Joins
            "join", "inner", "outer", "left", "right", "full", "cross", "apply", "on", "using",

            // Operators and predicates
            "and", "or", "not", "is", "null", "like", "in", "between", "exists", "any", "some", "escape",

            // Sorting
            "asc", "desc",

            // Expressions
            "case", "when", "then", "else", "end", "over", "partition",

            // Objects and constraints
            "table", "view", "index", "procedure", "proc", "function", "trigger", "schema", "database", "sequence",
            "primary", "foreign", "key", "references", "unique", "constraint", "default", "check", "identity",
            "clustered", "nonclustered", "cascade", "add", "column", "if",

            // Control flow and transactions
            "begin", "commit", "rollback", "transaction", "tran", "save", "while", "break", "continue", "return",
            "try", "catch", "throw", "raiserror",

            // Aggregate and common functions
            "count", "sum", "avg", "min", "max", "coalesce", "isnull", "nullif", "cast", "convert",
            "getdate", "getutcdate", "dateadd", "datediff", "row_number", "rank", "dense_rank",

            // Data types
            "int", "bigint", "smallint", "tinyint", "bit", "decimal", "numeric", "float", "real", "money",
            "char", "varchar", "nchar", "nvarchar", "binary", "varbinary", "datetime", "datetime2",
            "datetimeoffset", "uniqueidentifier", "integer", "boolean"
        };

        protected override bool IsKeyWord(string identifier)
        {
            return _keywords.Contains(identifier);
        }
    }
}
