using System;
using System.Collections.Generic;

namespace CommandPrompt
{
    public enum SqlTokenType
    {
        Whitespace,
        Keyword,
        Function,
        DataType,
        Identifier,
        QuotedIdentifier,
        Variable,
        GlobalVariable,
        TempTable,
        String,
        Number,
        Comment,
        Operator,
        Punctuation,
        BatchSeparator,
    }

    public struct SqlToken
    {
        public SqlToken(SqlTokenType type, int index, int length)
        {
            Type = type;
            Index = index;
            Length = length;
        }

        public SqlTokenType Type { get; }
        public int Index { get; }
        public int Length { get; }

        public override string ToString() => $"{Type}@{Index}+{Length}";
    }

    /// <summary>
    /// What the tokenizer is inside of at the end of a line, so that block comments, strings and
    /// bracketed identifiers can continue on the next line.
    /// </summary>
    public struct SqlLexState : IEquatable<SqlLexState>
    {
        public static readonly SqlLexState Normal = new SqlLexState(0, '\0');

        public SqlLexState(int commentDepth, char openQuote)
        {
            CommentDepth = commentDepth;
            OpenQuote = openQuote;
        }

        /// <summary>Nesting depth of /* */ comments (T-SQL allows nesting), 0 when not in a comment.</summary>
        public int CommentDepth { get; }

        /// <summary>The quote that opened an unterminated string ('), quoted identifier (") or bracket identifier ([); '\0' when none.</summary>
        public char OpenQuote { get; }

        public bool IsNormal => CommentDepth == 0 && OpenQuote == '\0';

        public bool Equals(SqlLexState other) => CommentDepth == other.CommentDepth && OpenQuote == other.OpenQuote;
        public override bool Equals(object obj) => obj is SqlLexState s && Equals(s);
        public override int GetHashCode() => CommentDepth * 397 ^ OpenQuote;
    }

    /// <summary>
    /// Line based lexer for generic SQL with the T-SQL extensions (variables, temp tables, [identifiers],
    /// N'strings', nested comments and GO). It only classifies text for highlighting and batch splitting;
    /// it does not validate syntax.
    /// </summary>
    public static class SqlTokenizer
    {
        public static List<SqlToken> TokenizeLine(string line, ref SqlLexState state)
        {
            var tokens = new List<SqlToken>();
            int i = 0;
            int n = line.Length;

            // Continue a construct that was left open on a previous line.
            if (state.CommentDepth > 0)
            {
                int end = ScanBlockComment(line, 0, state.CommentDepth, out int depth);
                tokens.Add(new SqlToken(SqlTokenType.Comment, 0, end));
                state = new SqlLexState(depth, '\0');
                i = end;
            }
            else if (state.OpenQuote != '\0')
            {
                char close = state.OpenQuote == '[' ? ']' : state.OpenQuote;
                int end = ScanQuoted(line, 0, close, out bool closed);
                tokens.Add(new SqlToken(state.OpenQuote == '\'' ? SqlTokenType.String : SqlTokenType.QuotedIdentifier, 0, end));
                state = closed ? SqlLexState.Normal : state;
                i = end;
            }

            if (i == 0 && state.IsNormal && IsBatchSeparatorLine(line, out int goStart, out int goLength))
            {
                if (goStart > 0)
                    tokens.Add(new SqlToken(SqlTokenType.Whitespace, 0, goStart));
                tokens.Add(new SqlToken(SqlTokenType.BatchSeparator, goStart, goLength));
                i = goStart + goLength;
                if (i < n)
                {
                    int rest = SkipWhitespace(line, i);
                    if (rest > i)
                        tokens.Add(new SqlToken(SqlTokenType.Whitespace, i, rest - i));
                    i = rest;
                    if (i < n)
                        tokens.Add(new SqlToken(SqlTokenType.Comment, i, n - i)); // only a -- comment can follow GO [count]
                }
                return tokens;
            }

            while (i < n && state.IsNormal)
            {
                char c = line[i];
                int start = i;

                if (char.IsWhiteSpace(c))
                {
                    i = SkipWhitespace(line, i);
                    tokens.Add(new SqlToken(SqlTokenType.Whitespace, start, i - start));
                }
                else if (c == '-' && Peek(line, i + 1) == '-')
                {
                    tokens.Add(new SqlToken(SqlTokenType.Comment, start, n - start));
                    i = n;
                }
                else if (c == '/' && Peek(line, i + 1) == '*')
                {
                    int end = ScanBlockComment(line, i + 2, 1, out int depth);
                    tokens.Add(new SqlToken(SqlTokenType.Comment, start, end - start));
                    state = new SqlLexState(depth, '\0');
                    i = end;
                }
                else if (c == '\'' || ((c == 'N' || c == 'n') && Peek(line, i + 1) == '\''))
                {
                    int quoteAt = c == '\'' ? i : i + 1;
                    int end = ScanQuoted(line, quoteAt + 1, '\'', out bool closed);
                    tokens.Add(new SqlToken(SqlTokenType.String, start, end - start));
                    if (!closed)
                        state = new SqlLexState(0, '\'');
                    i = end;
                }
                else if (c == '"' || c == '[' || c == '`')
                {
                    char close = c == '[' ? ']' : c;
                    int end = ScanQuoted(line, i + 1, close, out bool closed);
                    tokens.Add(new SqlToken(SqlTokenType.QuotedIdentifier, start, end - start));
                    if (!closed && c != '`')
                        state = new SqlLexState(0, c);
                    i = end;
                }
                else if (c == '@')
                {
                    bool global = Peek(line, i + 1) == '@';
                    i = ScanWord(line, i + (global ? 2 : 1));
                    tokens.Add(new SqlToken(global ? SqlTokenType.GlobalVariable : SqlTokenType.Variable, start, i - start));
                }
                else if (c == '#' && IsWordChar(Peek(line, i + 1)) || c == '#' && Peek(line, i + 1) == '#')
                {
                    i = ScanWord(line, i + (Peek(line, i + 1) == '#' ? 2 : 1));
                    tokens.Add(new SqlToken(SqlTokenType.TempTable, start, i - start));
                }
                else if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(line, i + 1))))
                {
                    i = ScanNumber(line, i);
                    tokens.Add(new SqlToken(SqlTokenType.Number, start, i - start));
                }
                else if (IsWordStart(c))
                {
                    i = ScanWord(line, i);
                    string word = line.Substring(start, i - start);
                    tokens.Add(new SqlToken(ClassifyWord(word, line, i), start, i - start));
                }
                else if (c == '(' || c == ')' || c == ',' || c == ';' || c == '.')
                {
                    i++;
                    tokens.Add(new SqlToken(SqlTokenType.Punctuation, start, 1));
                }
                else
                {
                    i++;
                    while (i < n && IsOperatorChar(line[i]) && !(line[i] == '-' && Peek(line, i + 1) == '-') && !(line[i] == '/' && Peek(line, i + 1) == '*'))
                        i++;
                    tokens.Add(new SqlToken(SqlTokenType.Operator, start, i - start));
                }
            }

            // A comment, string or identifier opened on this line and continuing past it.
            if (i < n)
                tokens.Add(new SqlToken(state.CommentDepth > 0 ? SqlTokenType.Comment : state.OpenQuote == '\'' ? SqlTokenType.String : SqlTokenType.QuotedIdentifier, i, n - i));

            return tokens;
        }

        /// <summary>
        /// True when the line is a batch separator: GO alone on the line, optionally followed by a repeat
        /// count and a -- comment. Only valid when the line does not start inside a comment or string.
        /// </summary>
        public static bool IsBatchSeparatorLine(string line, out int goStart, out int goLength)
        {
            return TryParseBatchSeparator(line, out goStart, out goLength, out _);
        }

        public static bool TryParseBatchSeparator(string line, out int goStart, out int goLength, out int repeatCount)
        {
            goStart = SkipWhitespace(line, 0);
            goLength = 0;
            repeatCount = 1;

            if (goStart + 2 > line.Length || string.Compare(line, goStart, "GO", 0, 2, StringComparison.OrdinalIgnoreCase) != 0)
                return false;

            int i = goStart + 2;
            if (i < line.Length && !char.IsWhiteSpace(line[i]) && !(line[i] == '-' && Peek(line, i + 1) == '-'))
                return false;

            int afterGo = i;
            i = SkipWhitespace(line, i);
            int countStart = i;
            while (i < line.Length && char.IsDigit(line[i]))
                i++;
            if (i > countStart)
            {
                if (!int.TryParse(line.Substring(countStart, i - countStart), out repeatCount) || repeatCount < 1)
                    return false;
                afterGo = i;
                i = SkipWhitespace(line, i);
            }

            if (i < line.Length && !(line[i] == '-' && Peek(line, i + 1) == '-'))
                return false;

            goLength = afterGo - goStart;
            return true;
        }

        private static SqlTokenType ClassifyWord(string word, string line, int end)
        {
            if (SqlKeywords.Functions.Contains(word))
            {
                // Only a function when called: "LEFT(" is a function, "LEFT JOIN" a keyword, and a
                // column named e.g. "Count" stays an identifier.
                int next = SkipWhitespace(line, end);
                if (next < line.Length && line[next] == '(')
                    return SqlTokenType.Function;
            }
            if (SqlKeywords.Keywords.Contains(word))
                return SqlTokenType.Keyword;
            if (SqlKeywords.DataTypes.Contains(word))
                return SqlTokenType.DataType;
            return SqlTokenType.Identifier;
        }

        private static int ScanBlockComment(string line, int i, int depth, out int newDepth)
        {
            while (i < line.Length)
            {
                if (line[i] == '*' && Peek(line, i + 1) == '/')
                {
                    i += 2;
                    if (--depth == 0)
                    {
                        newDepth = 0;
                        return i;
                    }
                }
                else if (line[i] == '/' && Peek(line, i + 1) == '*')
                {
                    i += 2;
                    depth++;
                }
                else
                {
                    i++;
                }
            }
            newDepth = depth;
            return line.Length;
        }

        /// <summary>Scans to just past the closing quote; a doubled closing quote is an escaped quote.</summary>
        private static int ScanQuoted(string line, int i, char close, out bool closed)
        {
            while (i < line.Length)
            {
                if (line[i] == close)
                {
                    if (Peek(line, i + 1) == close)
                    {
                        i += 2;
                        continue;
                    }
                    closed = true;
                    return i + 1;
                }
                i++;
            }
            closed = false;
            return line.Length;
        }

        private static int ScanNumber(string line, int i)
        {
            if (line[i] == '0' && (Peek(line, i + 1) == 'x' || Peek(line, i + 1) == 'X'))
            {
                i += 2;
                while (i < line.Length && Uri.IsHexDigit(line[i]))
                    i++;
                return i;
            }

            while (i < line.Length && (char.IsDigit(line[i]) || line[i] == '.'))
                i++;
            if (i < line.Length && (line[i] == 'e' || line[i] == 'E'))
            {
                int j = i + 1;
                if (j < line.Length && (line[j] == '+' || line[j] == '-'))
                    j++;
                if (j < line.Length && char.IsDigit(line[j]))
                {
                    i = j;
                    while (i < line.Length && char.IsDigit(line[i]))
                        i++;
                }
            }
            return i;
        }

        private static int ScanWord(string line, int i)
        {
            while (i < line.Length && IsWordChar(line[i]))
                i++;
            return i;
        }

        private static int SkipWhitespace(string line, int i)
        {
            while (i < line.Length && char.IsWhiteSpace(line[i]))
                i++;
            return i;
        }

        private static char Peek(string line, int i) => i < line.Length ? line[i] : '\0';
        private static bool IsWordStart(char c) => char.IsLetter(c) || c == '_';
        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '$' || c == '#' || c == '@';
        private static bool IsOperatorChar(char c) => "+-*/%=<>!&|^~:".IndexOf(c) >= 0;
    }
}
