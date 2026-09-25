using Common;
using SqlExecute;
using System;
using System.Collections.Generic;

namespace SqlStudio
{
    public class DatabaseKeywordEscapeManager : IDatabaseKeywordEscape
    {
        // SQL Server reserved keywords (https://learn.microsoft.com/sql/t-sql/language-elements/reserved-keywords-transact-sql)
        // plus a few words reserved in other databases. Table/column names matching these are escaped as [name].
        private readonly HashSet<string> _keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "ADD", "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "AUTHORIZATION",
                "BACKUP", "BEGIN", "BETWEEN", "BREAK", "BROWSE", "BULK", "BY",
                "CASCADE", "CASE", "CHECK", "CHECKPOINT", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE", "COLUMN", "COMMIT",
                "COMPUTE", "CONSTRAINT", "CONTAINS", "CONTAINSTABLE", "CONTINUE", "CONVERT", "CREATE", "CROSS", "CURRENT",
                "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR",
                "DATABASE", "DBCC", "DEALLOCATE", "DECLARE", "DEFAULT", "DELETE", "DENY", "DESC", "DISK", "DISTINCT",
                "DISTRIBUTED", "DOUBLE", "DROP", "DUMP",
                "ELSE", "END", "ERRLVL", "ESCAPE", "EXCEPT", "EXEC", "EXECUTE", "EXISTS", "EXIT", "EXTERNAL",
                "FETCH", "FILE", "FILLFACTOR", "FOR", "FOREIGN", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FUNCTION",
                "GOTO", "GRANT", "GROUP",
                "HAVING", "HOLDLOCK",
                "IDENTITY", "IDENTITY_INSERT", "IDENTITYCOL", "IF", "IN", "INDEX", "INNER", "INSERT", "INTERSECT", "INTO", "IS",
                "JOIN",
                "KEY", "KILL",
                "LEFT", "LIKE", "LINENO", "LOAD",
                "MERGE",
                "NATIONAL", "NOCHECK", "NONCLUSTERED", "NOT", "NULL", "NULLIF",
                "OF", "OFF", "OFFSETS", "ON", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML", "OPTION", "OR",
                "ORDER", "OUTER", "OVER",
                "PERCENT", "PIVOT", "PLAN", "PRECISION", "PRIMARY", "PRINT", "PROC", "PROCEDURE", "PUBLIC",
                "RAISERROR", "READ", "READTEXT", "RECONFIGURE", "REFERENCES", "REPLICATION", "RESTORE", "RESTRICT", "RETURN",
                "REVERT", "REVOKE", "RIGHT", "ROLLBACK", "ROWCOUNT", "ROWGUIDCOL", "RULE",
                "SAVE", "SCHEMA", "SECURITYAUDIT", "SELECT", "SEMANTICKEYPHRASETABLE", "SEMANTICSIMILARITYDETAILSTABLE",
                "SEMANTICSIMILARITYTABLE", "SESSION_USER", "SET", "SETUSER", "SHUTDOWN", "SOME", "STATISTICS", "SYSTEM_USER",
                "TABLE", "TABLESAMPLE", "TEXTSIZE", "THEN", "TO", "TOP", "TRAN", "TRANSACTION", "TRIGGER", "TRUNCATE",
                "TRY_CONVERT", "TSEQUAL",
                "UNION", "UNIQUE", "UNPIVOT", "UPDATE", "UPDATETEXT", "USE", "USER",
                "VALUES", "VARYING", "VIEW",
                "WAITFOR", "WHEN", "WHERE", "WHILE", "WITH", "WRITETEXT",

                // Reserved in other databases (SQLite, PostgreSQL, MySQL)
                "LIMIT", "OFFSET", "RETURNING", "WINDOW"
            };

        /// <summary>
        /// Returns the provider of the current connection, used to select the identifier quote characters.
        /// </summary>
        public Func<SqlExecuter.DatabaseProvider> ProviderSource { get; set; }

        public string EscapeObject(string value)
        {
            if (value != null && _keywords.Contains(value))
            {
                var provider = ProviderSource?.Invoke() ?? SqlExecuter.DatabaseProvider.SQLSERVER;
                switch (provider)
                {
                    case SqlExecuter.DatabaseProvider.POSTGRESQL:
                    case SqlExecuter.DatabaseProvider.ORACLE:
                        return $"\"{value}\"";
                    case SqlExecuter.DatabaseProvider.MySql:
                        return $"`{value}`";
                    default:
                        return $"[{value}]";
                }
            }

            return value;
        }
    }
}
