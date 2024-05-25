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
            "TRUNCATE",
            "INSERT INTO",
            "CREATE TABLE"
        };

        private List<string> _sqlKeywordsSearch = new List<string>
        {
            "GROUP BY",
            "ORDER BY",
            "AND",
            "OR",
            "GETDATE()",
            "GETUTCDATE()",
            "CONVERT(",
            "DATEADD(day, ",
            "DATEADD(month, ",
            "DATEADD(year, "
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
            var symbol = symbols[symbolIndex];
            if (symbol.Index == 0)
            {
                return MergePossible(_sqlKeywordsStart, symbol);
            }

            var keyWord = GetPreviousSymbol(symbols, _sqlKeywords, index);
            if (keyWord == null)
            {
                return MergePossible(_sqlKeywords, symbol);
            }

            //table list
            if (keyWord.Text.Equals("UPDATE", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableNames();
                tables.Insert(0, "SET");

                return MergePossible(tables, symbol);
            }

            //table list
            if (keyWord.Text.Equals("FROM", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableNames();
                tables.Insert(0, "WHERE");
                tables.Insert(0, "LEFT JOIN");
                tables.Insert(0, "FULL OUTER JOIN");
                tables.Insert(0, "JOIN");
                tables.Insert(0, "RIGHT JOIN");

                return MergePossible(tables, symbol);
            }

            if (keyWord.Text.Equals("JOIN", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableNames();
                tables.Insert(0, "ON");

                return MergePossible(tables, symbol);
            }

            if (keyWord.Text.Equals("INTO", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableNames();

                return MergePossible(tables, symbol);
            }

            //column list
            if (keyWord.Text.Equals("SELECT", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableInfo(symbols);
                var columns = GetColumnNames(symbol.Text.Length == 0, tables, symbol);
                columns.Insert(0, "FROM");

                return MergePossible(columns, symbol);
            }

            //column list
            if (keyWord.Text.Equals("SET", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableInfo(symbols);
                var columns = GetColumnNames(symbol.Text.Length == 0, tables, symbol);
                columns.Insert(0, "WHERE");

                return MergePossible(columns, symbol);
            }

            //column list
            if (keyWord.Text.Equals("WHERE", StringComparison.CurrentCultureIgnoreCase) ||
                keyWord.Text.Equals("ON", StringComparison.CurrentCultureIgnoreCase))
            {
                var tables = GetTableInfo(symbols);
                var columns = GetColumnNames(symbol.Text.Length == 0, tables, symbol);
                columns.AddRange(_sqlKeywordsSearch);

                return MergePossible(columns, symbol);
            }

            return MergePossible(_sqlKeywords, symbol);
        }

        
        public CommandCompletionResult MergePossible(List<string> possibleCompletions, Symbol symbol)
        {
            var ret = new CommandCompletionResult { CompletedText = symbol.Text, CompletedTextStartIndex = symbol.Index };
            foreach (var item in possibleCompletions)
            {
                if (item.IndexOf(symbol.Text, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ret.PossibleCompletions.Add(item);
                }
                else if (item.IndexOf($"[{symbol.Text}", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ret.PossibleCompletions.Add(item);
                }
            }
            return ret;
        }

        private void InsertEmptySymbolIfIndexBetweenSpaces(List<Symbol> symbols, int index)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].Index == index && (symbols[i].Text == " " || symbols[i].Text == ","))
                {
                    if (i > 0 && symbols[i - 1].Index == (index - 1) && symbols[i - 1].Text == " " || symbols[i - 1].Text == ",")
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
                if (c != ' ' && c != ',')
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
                    
                    ret.Add(new Symbol { Index = i, Text = c.ToString() });
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

        public int GetIndex(List<Symbol> symbols, int index)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols[i].Index <= index && (symbols[i].Index + symbols[i].Length) >= index && symbols[i].Text != " " && symbols[i].Text != ",")
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

            int index = GetIndex(symbols, "FROM");
            if (index >= 0)
            {
                ret.AddRange(GetTableInfo(symbols, index + 1));
            }

            index = GetIndex(symbols, "JOIN");
            if (index >= 0)
            {
                ret.AddRange(GetTableInfo(symbols, index + 1));
            }

            index = GetIndex(symbols, "UPDATE");
            if (index >= 0)
            {
                ret.AddRange(GetTableInfo(symbols, index + 1));
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

                while (i < symbols.Count && 
                    _sqlKeywords.FirstOrDefault(x => x.Equals(symbols[i].Text, StringComparison.OrdinalIgnoreCase)) == null && 
                    symbols[i].Text != ",")
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
            int i = index;
            while (i < symbols.Count && symbols[i].Text == " ")
            {
                i++;
            }

            if (i < symbols.Count)
            {
                CommandCompleteTableInfo ti = new CommandCompleteTableInfo { TableName = symbols[i].Text };
                i++;
                while (i < symbols.Count && symbols[i].Text == " ")
                {
                    i++;
                }
                if (i < symbols.Count && _sqlKeywords.FirstOrDefault(x => x.Equals(symbols[i].Text, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    ti.Alias = symbols[i].Text;
                }
                return ti;
            }

            return null;
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
                    var tableInfo = _databaseSchemaInfo.Tables.FirstOrDefault(x => x.TableName.Equals(table.TableName.Replace("[", "").Replace("]", ""), StringComparison.CurrentCultureIgnoreCase));
                    if ((tableAlias == table.Alias) || tableAlias == "")
                    {
                        if (tableInfo?.Columns != null)
                        {
                            foreach (var column in tableInfo.Columns)
                            {
                                var columnName = tableAlias != "" ? tableAlias + "." + column.ColumnName : column.ColumnName;
                                columnName = _databaseKeywordEscape.EscapeObject(columnName);
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
