using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlCommandCompleter
{
    public class Symbol
    {
        public string Text { get; set; } = String.Empty;
        public int Index { get; set; }
        public int Length => Text.Length;

        public override string ToString()
        {
            return $"Index: {Index}: {Text}";
        }
    }

    public class SqlCompleter : ISqlCompleter
    {
        private readonly ILogger _logger;
        private readonly IDatabaseSchemaInfo _databaseSchemaInfo;
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;

        // Characters that separate symbols. Whitespace is stored as a " " symbol, the others as their own symbol.
        private static readonly char[] _separatorChars = { ' ', ',', '\t', '\r', '\n', '(', ')', '=', '<', '>', '!' };

        private List<string> _sqlKeywords = new List<string>
        {
            "SELECT",
            "UPDATE",
            "DELETE",
            "TRUNCATE",
            "INSERT INTO",
            "CREATE TABLE",
            "FROM",
            "WHERE",
            "SET",
            "GROUP BY",
            "ORDER BY",
            "JOIN",
            "LEFT",
            "FULL",
            "OUTER",
            "ON",
            "INTO",
            "VALUES"
        };

        private List<string> _sqlKeywordsStart = new List<string>
        {
            "SELECT",
            "UPDATE",
            "DELETE",
            "TRUNCATE TABLE",
            "INSERT INTO",
            "CREATE TABLE",
            "CREATE VIEW",
            "CREATE INDEX",
            "ALTER TABLE",
            "DROP TABLE",
            "WITH",
            "EXEC",
            "BEGIN TRANSACTION",
            "COMMIT",
            "ROLLBACK"
        };

        private List<string> _sqlKeywordsSelect = new List<string>
        {
            "DISTINCT",
            "TOP",
            "CASE WHEN",
            "AS"
        };

        private List<string> _sqlFunctions = new List<string>
        {
            "COUNT(",
            "SUM(",
            "AVG(",
            "MIN(",
            "MAX(",
            "COALESCE(",
            "ISNULL(",
            "CAST(",
            "CONVERT(",
            "UPPER(",
            "LOWER(",
            "LEN(",
            "SUBSTRING(",
            "DATEADD(",
            "DATEDIFF(",
            "GETDATE()",
            "GETUTCDATE()",
            "ROW_NUMBER() OVER ("
        };

        private List<string> _sqlKeywordsSearch = new List<string>
        {
            "NULL",
            "NOT",
            "EXISTS (",
            "GETDATE()",
            "GETUTCDATE()",
            "CONVERT(",
            "DATEADD(day, ",
            "DATEADD(month, ",
            "DATEADD(year, "
        };

        private List<string> _sqlOperators = new List<string>
        {
            "=",
            "<>",
            "<",
            ">",
            "<=",
            ">=",
            "LIKE",
            "NOT LIKE",
            "IN (",
            "NOT IN (",
            "IS NULL",
            "IS NOT NULL",
            "BETWEEN",
            "AND",
            "OR"
        };

        private List<string> _sqlKeywordsAfterFromTable = new List<string>
        {
            "WHERE",
            "JOIN",
            "INNER JOIN",
            "LEFT JOIN",
            "RIGHT JOIN",
            "FULL OUTER JOIN",
            "CROSS JOIN",
            "GROUP BY",
            "ORDER BY"
        };

        private List<string> _sqlValues = new List<string>
        {
            "NULL",
            "GETDATE()",
            "GETUTCDATE()"
        };

        // Keywords that decide what kind of completion is possible after them
        private static readonly HashSet<string> _contextKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "FROM", "WHERE", "SET", "UPDATE", "DELETE", "INTO", "VALUES", "JOIN", "ON", "BY", "HAVING", "TRUNCATE", "TABLE"
        };

        // Words that can not be a table name or alias
        private static readonly HashSet<string> _reservedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "FROM", "WHERE", "SET", "UPDATE", "DELETE", "INSERT", "INTO", "VALUES", "JOIN", "INNER", "LEFT", "RIGHT", "FULL",
            "OUTER", "CROSS", "ON", "AND", "OR", "NOT", "GROUP", "ORDER", "BY", "HAVING", "AS", "UNION", "TOP", "DISTINCT", "TRUNCATE",
            "TABLE", "LIMIT", "OFFSET"
        };

        // Symbols after which a column or value is expected in a search condition
        private static readonly HashSet<string> _operandExpectedAfter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "WHERE", "ON", "HAVING", "AND", "OR", "NOT", "LIKE", "IN", "BETWEEN", "EXISTS", ",", "(", "=", "<", ">", "!"
        };

        public SqlCompleter(ILogger logger, IDatabaseSchemaInfo databaseSchemaInfo, IDatabaseKeywordEscape databaseKeywordEscape)
        {
            _logger = logger;
            _databaseSchemaInfo = databaseSchemaInfo;
            _databaseKeywordEscape = databaseKeywordEscape;
        }

        public CommandCompletionResult GetPossibleCompletions(string sqlCommand, int index)
        {
            if (sqlCommand == null)
            {
                throw new ArgumentException(nameof(sqlCommand));
            }

            sqlCommand = sqlCommand.TrimEnd(new char[] { ';' });

            if (index > sqlCommand.Length)
            {
                throw new ArgumentException(nameof(index));
            }

            var symbols = GetSymbols(sqlCommand);
            InsertEmptySymbolIfIndexBetweenSpaces(symbols, index);
            var symbolIndex = GetIndex(symbols, index);
            if (symbolIndex < 0)
            {
                return new CommandCompletionResult { CompletedText = "", CompletedTextStartIndex = index };
            }

            var symbol = symbols[symbolIndex];
            var prevIndex = GetPreviousNonWhitespaceIndex(symbols, symbolIndex);
            if (prevIndex < 0)
            {
                return MergePossible(_sqlKeywordsStart, symbol);
            }

            var contextIndex = GetContextKeywordIndex(symbols, symbolIndex);
            if (contextIndex < 0)
            {
                return MergePossible(_sqlKeywords, symbol);
            }

            var prev = symbols[prevIndex];
            bool afterContextKeyword = prevIndex == contextIndex;
            bool afterComma = prev.Text == ",";
            var context = GetContextName(symbols, contextIndex);

            switch (context)
            {
                case "SELECT":
                {
                    var columns = GetColumnNames(symbol.Text.Length == 0, GetTableInfo(symbols), symbol);
                    columns.Insert(0, "FROM");
                    columns.AddRange(_sqlKeywordsSelect);
                    columns.AddRange(_sqlFunctions);
                    return MergePossible(columns, symbol);
                }

                case "FROM":
                    if (afterContextKeyword || afterComma)
                    {
                        return MergePossible(GetTableNames(), symbol);
                    }
                    if (IsDeleteStatement(symbols))
                    {
                        return MergePossible(new List<string> { "WHERE" }, symbol);
                    }
                    return MergePossible(_sqlKeywordsAfterFromTable, symbol);

                case "JOIN":
                    if (afterContextKeyword)
                    {
                        return MergePossible(GetTableNames(), symbol);
                    }
                    return MergePossible(new List<string> { "ON" }, symbol);

                case "UPDATE":
                    if (afterContextKeyword)
                    {
                        return MergePossible(GetTableNames(), symbol);
                    }
                    return MergePossible(new List<string> { "SET" }, symbol);

                case "INTO":
                    if (afterContextKeyword)
                    {
                        return MergePossible(GetTableNames(), symbol);
                    }
                    if (IsInsideParentheses(symbols, contextIndex, symbolIndex))
                    {
                        return MergePossible(GetColumnNames(false, GetTableInfo(symbols), symbol), symbol);
                    }
                    if (prev.Text == ")")
                    {
                        return MergePossible(new List<string> { "VALUES", "SELECT" }, symbol);
                    }
                    return MergePossible(new List<string> { "(", "VALUES", "SELECT" }, symbol);

                case "VALUES":
                    return MergePossible(_sqlValues, symbol);

                case "SET":
                {
                    if (afterContextKeyword || afterComma)
                    {
                        return MergePossible(GetColumnNames(false, GetTableInfo(symbols), symbol), symbol);
                    }
                    if (prev.Text == "=")
                    {
                        var values = GetColumnNames(false, GetTableInfo(symbols), symbol);
                        values.AddRange(_sqlKeywordsSearch);
                        return MergePossible(values, symbol);
                    }
                    return MergePossible(new List<string> { "WHERE" }, symbol);
                }

                case "WHERE":
                case "ON":
                case "HAVING":
                    return GetSearchConditionCompletions(context, symbols, prev, symbol);

                case "ORDER BY":
                    if (afterContextKeyword || afterComma)
                    {
                        return MergePossible(GetColumnNames(false, GetTableInfo(symbols), symbol), symbol);
                    }
                    return MergePossible(new List<string> { "ASC", "DESC" }, symbol);

                case "GROUP BY":
                    if (afterContextKeyword || afterComma)
                    {
                        return MergePossible(GetColumnNames(false, GetTableInfo(symbols), symbol), symbol);
                    }
                    return MergePossible(new List<string> { "HAVING", "ORDER BY" }, symbol);

                case "DELETE":
                    return MergePossible(new List<string> { "FROM" }, symbol);

                case "TRUNCATE":
                    return MergePossible(new List<string> { "TABLE" }, symbol);

                case "TRUNCATE TABLE":
                case "DROP TABLE":
                case "ALTER TABLE":
                    if (afterContextKeyword)
                    {
                        return MergePossible(GetTableNames(), symbol);
                    }
                    return MergePossible(new List<string>(), symbol);
            }

            if (context == "TABLE" || context.EndsWith(" TABLE"))
            {
                // e.g. CREATE TABLE <name>, nothing sensible to complete
                return MergePossible(new List<string>(), symbol);
            }

            return MergePossible(_sqlKeywords, symbol);
        }

        private CommandCompletionResult GetSearchConditionCompletions(string context, List<Symbol> symbols, Symbol prev, Symbol symbol)
        {
            if (prev.Text.Equals("IS", StringComparison.OrdinalIgnoreCase))
            {
                return MergePossible(new List<string> { "NULL", "NOT NULL" }, symbol);
            }

            if (_operandExpectedAfter.Contains(prev.Text))
            {
                var operands = GetColumnNames(false, GetTableInfo(symbols), symbol);
                if (context == "HAVING")
                {
                    operands.AddRange(_sqlFunctions);
                }
                operands.AddRange(_sqlKeywordsSearch);
                return MergePossible(operands, symbol);
            }

            var keywords = new List<string>(_sqlOperators);
            switch (context)
            {
                case "WHERE":
                    keywords.Add("GROUP BY");
                    keywords.Add("ORDER BY");
                    break;
                case "ON":
                    keywords.Add("WHERE");
                    keywords.AddRange(_sqlKeywordsAfterFromTable.Where(x => x.EndsWith("JOIN")));
                    keywords.Add("GROUP BY");
                    keywords.Add("ORDER BY");
                    break;
                case "HAVING":
                    keywords.Add("ORDER BY");
                    break;
            }
            return MergePossible(keywords, symbol);
        }

        public CommandCompletionResult MergePossible(List<string> possibleCompletions, Symbol symbol)
        {
            var ret = new CommandCompletionResult { CompletedText = symbol.Text, CompletedTextStartIndex = symbol.Index };
            var unquotedText = RemoveIdentifierQuotes(symbol.Text);
            foreach (var item in possibleCompletions)
            {
                if (item.IndexOf(symbol.Text, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ret.PossibleCompletions.Add(item);
                }
                else if (RemoveIdentifierQuotes(item).IndexOf(unquotedText, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // match escaped names like [key], "key", `key` and alias.[key] without typing the quotes
                    ret.PossibleCompletions.Add(item);
                }
            }
            return ret;
        }

        private static string RemoveIdentifierQuotes(string text)
        {
            return text.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("`", "");
        }

        private static bool IsSeparator(Symbol symbol)
        {
            return symbol.Text.Length == 1 && _separatorChars.Contains(symbol.Text[0]);
        }

        private static bool IsWhitespace(Symbol symbol)
        {
            return symbol.Text == " ";
        }

        private void InsertEmptySymbolIfIndexBetweenSpaces(List<Symbol> symbols, int index)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].Index == index && IsSeparator(symbols[i]))
                {
                    if (i == 0 || IsSeparator(symbols[i - 1]))
                    {
                        symbols.Insert(i, new Symbol { Index = symbols[i].Index, Text = "" });
                    }
                    break;
                }
            }
        }

        public List<Symbol> GetSymbols(string text)
        {
            var ret = new List<Symbol>();
            int index = 0;
            string currentSymbolText = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!_separatorChars.Contains(c))
                {
                    currentSymbolText += c;
                    if (i == text.Length - 1)
                    {
                        ret.Add(new Symbol { Index = index, Text = currentSymbolText });
                    }
                }
                else
                {
                    if (currentSymbolText.Length > 0)
                    {
                        ret.Add(new Symbol { Index = index, Text = currentSymbolText });
                    }

                    ret.Add(new Symbol { Index = i, Text = char.IsWhiteSpace(c) ? " " : c.ToString() });
                    index = i + 1;
                    currentSymbolText = "";
                }
            }
            ret.Add(new Symbol { Index = text.Length, Text = "" });
            return ret;
        }

        public Symbol GetSymbol(string text, int index)
        {
            var symbols = GetSymbols(text);
            int i = GetIndex(symbols, index);
            return symbols[i];
        }

        public Symbol GetPreviousSymbol(List<Symbol> symbols, List<string> possibleKeywords, int index)
        {
            int symIndex = GetIndex(symbols, index);
            if (symIndex == -1)
                return null;

            for (int i = symIndex; i >= 0; i--)
            {
                foreach (var keyWord in possibleKeywords)
                {
                    if (symbols[i].Text.Equals(keyWord, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return symbols[i];
                    }
                }
            }

            return null;
        }

        private int GetPreviousNonWhitespaceIndex(List<Symbol> symbols, int symbolIndex)
        {
            for (int i = symbolIndex - 1; i >= 0; i--)
            {
                if (!IsWhitespace(symbols[i]) && symbols[i].Length > 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetContextKeywordIndex(List<Symbol> symbols, int symbolIndex)
        {
            for (int i = symbolIndex - 1; i >= 0; i--)
            {
                if (!IsSeparator(symbols[i]) && _contextKeywords.Contains(symbols[i].Text))
                {
                    return i;
                }
            }
            return -1;
        }

        // Returns the context keyword in upper case, combined with the word before it for BY (ORDER BY / GROUP BY) and TABLE (TRUNCATE TABLE etc.)
        private string GetContextName(List<Symbol> symbols, int contextIndex)
        {
            var context = symbols[contextIndex].Text.ToUpperInvariant();
            if (context == "BY" || context == "TABLE")
            {
                int prevIndex = GetPreviousNonWhitespaceIndex(symbols, contextIndex);
                if (prevIndex >= 0)
                {
                    return symbols[prevIndex].Text.ToUpperInvariant() + " " + context;
                }
            }
            return context;
        }

        private bool IsDeleteStatement(List<Symbol> symbols)
        {
            var first = symbols.FirstOrDefault(x => !IsSeparator(x) && x.Length > 0);
            return first != null && first.Text.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsInsideParentheses(List<Symbol> symbols, int fromIndex, int toIndex)
        {
            int depth = 0;
            for (int i = fromIndex + 1; i < toIndex; i++)
            {
                if (symbols[i].Text == "(")
                    depth++;
                else if (symbols[i].Text == ")")
                    depth--;
            }
            return depth > 0;
        }

        public int GetIndex(List<Symbol> symbols, int index)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].Index <= index && (symbols[i].Index + symbols[i].Length) >= index && !IsSeparator(symbols[i]))
                {
                    return i;
                }
                else if (symbols[i].Index == index && symbols[i].Length == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetIndex(List<Symbol> symbols, string text)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].Text.Equals(text, StringComparison.CurrentCultureIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        public List<CommandCompleteTableInfo> GetTableInfo(List<Symbol> symbols)
        {
            var ret = new List<CommandCompleteTableInfo>();

            for (int i = 0; i < symbols.Count; i++)
            {
                var text = symbols[i].Text;
                if (text.Equals("FROM", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("JOIN", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("UPDATE", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("INTO", StringComparison.OrdinalIgnoreCase))
                {
                    ret.AddRange(GetTableInfo(symbols, i + 1));
                }
            }

            return ret;
        }

        private List<CommandCompleteTableInfo> GetTableInfo(List<Symbol> symbols, int index)
        {
            var ret = new List<CommandCompleteTableInfo>();
            int i = index;
            while (i < symbols.Count)
            {
                var tableInfo = GetSingleTableInfo(symbols, i);
                if (tableInfo != null)
                {
                    ret.Add(tableInfo);
                }

                // skip table name and alias, stop at next keyword or separator (other than whitespace)
                while (i < symbols.Count &&
                    !_reservedWords.Contains(symbols[i].Text) &&
                    (!IsSeparator(symbols[i]) || IsWhitespace(symbols[i])))
                {
                    i++;
                }
                if (i < symbols.Count && symbols[i].Text == ",")
                {
                    i++;
                }
                else
                {
                    break;
                }
            }

            return ret;
        }

        private CommandCompleteTableInfo GetSingleTableInfo(List<Symbol> symbols, int index)
        {
            int i = SkipWhitespace(symbols, index);
            if (i >= symbols.Count || !IsName(symbols[i]))
            {
                return null;
            }

            CommandCompleteTableInfo ti = new CommandCompleteTableInfo { TableName = symbols[i].Text };
            i = SkipWhitespace(symbols, i + 1);
            if (i < symbols.Count && symbols[i].Text.Equals("AS", StringComparison.OrdinalIgnoreCase))
            {
                i = SkipWhitespace(symbols, i + 1);
            }
            if (i < symbols.Count && IsName(symbols[i]))
            {
                ti.Alias = symbols[i].Text;
            }
            return ti;
        }

        private static int SkipWhitespace(List<Symbol> symbols, int index)
        {
            while (index < symbols.Count && IsWhitespace(symbols[index]))
            {
                index++;
            }
            return index;
        }

        private static bool IsName(Symbol symbol)
        {
            return symbol.Length > 0 && !IsSeparator(symbol) && !_reservedWords.Contains(symbol.Text);
        }

        private List<string> GetTableNames()
        {
            List<string> ret = new List<string>();
            foreach (var table in _databaseSchemaInfo.Tables)
            {
                var tableName = _databaseKeywordEscape.EscapeObject(table.TableName);
                ret.Add(tableName);
            }
            return ret;
        }

        private List<string> GetColumnNames(bool includeStar, List<CommandCompleteTableInfo> possibleTables, Symbol symbol)
        {
            List<string> ret = new List<string>();
            if (includeStar)
            {
                ret.Add("*");
            }

            var tableAlias = GetColumnAlias(symbol);

            if (possibleTables.Count > 0)
            {
                foreach (var table in possibleTables)
                {
                    var tableInfo = _databaseSchemaInfo.Tables.FirstOrDefault(x => x.TableName.Equals(RemoveIdentifierQuotes(table.TableName), StringComparison.CurrentCultureIgnoreCase));
                    if ((tableAlias == table.Alias) || tableAlias == "")
                    {
                        if (tableInfo?.Columns != null)
                        {
                            foreach (var column in tableInfo.Columns)
                            {
                                var columnName = _databaseKeywordEscape.EscapeObject(column.ColumnName);
                                if (tableAlias != "")
                                {
                                    columnName = tableAlias + "." + columnName;
                                }
                                if (!ret.Contains(columnName))
                                {
                                    ret.Add(columnName);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var table in _databaseSchemaInfo.Tables)
                {
                    foreach (var column in table.Columns)
                    {
                        var columnName = _databaseKeywordEscape.EscapeObject(column.ColumnName);
                        if (!ret.Contains(columnName))
                        {
                            ret.Add(columnName);
                        }
                    }
                }
            }

            return ret;
        }

        private string GetColumnAlias(Symbol symbol)
        {
            int index = symbol.Text.IndexOf('.');
            if (index < 0)
                return "";
            return symbol.Text.Substring(0, index);
        }
    }
}
