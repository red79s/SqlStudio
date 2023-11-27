using Common;
using System;
using System.Collections.Generic;

namespace SqlStudio
{
    public class DatabaseKeywordEscapeManager : IDatabaseKeywordEscape
    {
        private List<string> _keywords = new List<string>
            {
                "SELECT",
                "UPDATE",
                "DELETE",
                "TRUNCATE",
                "CREATE TABLE",
                "CREATE",
                "TABLE",
                "FROM",
                "WHERE",
                "SET",
                "GROUP BY",
                "ORDER BY",
                "ORDER",
                "GROUP",
                "BY",
                "JOIN",
                "LEFT",
                "FULL",
                "OUTER",
                "PRIMARY KEY",
                "PRIMARY",
                "KEY"
            };

        public string EscapeObject(string value)
        {
            foreach (var keyword in _keywords)
            {
                if (keyword.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return $"[{value}]";
                }
            }

            return value;
        }
    }
}
