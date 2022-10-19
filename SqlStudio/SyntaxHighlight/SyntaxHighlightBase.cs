using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SqlStudio.SyntaxHighlight
{
    public enum TokenType { IDENTIFIER, KEYWORD, MATH, PARENTHESIS_LEFT, PARENTHESIS_RIGHT, BRACKET_LEFT, BRACKET_RIGHT, LITERAL_STRING, LITERAL_INT, LITERNAL_NUMBERIC, UNKNOWN };

    class SyntaxHighlightBase
    {
        private string _regexExpression = "(\"([^\\\\\"]|\\\\.)*\")|('([^']*)')|([^ ;,<>.\\[\\]\\(\\)]+)|(;)|(,)|(<)|(>)|(.)|(\\.)|(\\[)|(\\])|(\\()|(\\))|(<)|(>)";
        private Regex _regex = null;
        private Color _colorStrings = Color.Red;
        private Color _colorNumbers = Color.Green;
        private Color _colorKeyWords = Color.Blue;
        private Color _colorIdentifiers = Color.Black;
        private Color _colorDefault = Color.Black;

        public SyntaxHighlightBase()
        {
            this._regex = new Regex(_regexExpression, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public List<Token> GetTokens(string text)
        {
            List<Token> ret = new List<Token>();
            string[] lines = text.Replace("\r", "").Split(new char[] {'\n'});
            int lineIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                MatchCollection matches = this._regex.Matches(lines[i]);

                foreach (Match match in matches)
                {
                    if (match.Value.Length > 0)
                    {
                        Token tok = new Token(match.Value, TokenType.UNKNOWN, i, match.Index);
                        tok.LineIndex = lineIndex + match.Index;
                        ret.Add(tok);
                    }
                }
            }

            this.GetTokenTypes(ref ret);

            return ret;
        }

        private void GetTokenTypes(ref List<Token> tokens)
        {
            int iTest = 0;
            float fTest = 0;

            foreach (Token token in tokens)
            {
                if (IsKeyWord(token.Value))
                {
                    token.TokenType = TokenType.KEYWORD;
                    token.Color = this._colorKeyWords;
                }
                else if (token.Value[0] == '"' && token.Value[token.Value.Length - 1] == '"')
                {
                    token.TokenType = TokenType.LITERAL_STRING;
                    token.Color = this._colorStrings;
                }
                else if (token.Value[0] == '\'' && token.Value[token.Value.Length - 1] == '\'')
                {
                    token.TokenType = TokenType.LITERAL_STRING;
                    token.Color = this._colorStrings;
                }
                else if (token.Value == "-" || token.Value == "+" || token.Value == "+=" || token.Value == "-=")
                {
                    token.TokenType = TokenType.MATH;
                    token.Color = this._colorDefault;
                }
                else if (token.Value == "(")
                {
                    token.TokenType = TokenType.PARENTHESIS_LEFT;
                    token.Color = this._colorDefault;
                }
                else if (token.Value == ")")
                {
                    token.TokenType = TokenType.PARENTHESIS_RIGHT;
                    token.Color = this._colorDefault;
                }
                else if (token.Value == "[")
                {
                    token.TokenType = TokenType.BRACKET_LEFT;
                    token.Color = this._colorDefault;
                }
                else if (token.Value == "]")
                {
                    token.TokenType = TokenType.BRACKET_RIGHT;
                    token.Color = this._colorDefault;
                }
                else if (int.TryParse(token.Value, out iTest))
                {
                    token.TokenType = TokenType.LITERAL_INT;
                    token.Color = this._colorNumbers;
                }
                else if (float.TryParse(token.Value, out fTest))
                {
                    token.TokenType = TokenType.LITERNAL_NUMBERIC;
                    token.Color = this._colorNumbers;
                }
                else
                {
                    token.TokenType = TokenType.IDENTIFIER;
                    token.Color = this._colorIdentifiers;
                }
            }
        }

        protected virtual bool IsKeyWord(string identifier)
        {
            return false;
        }

        public Color DefaultColor
        {
            get { return this._colorDefault; }
            set { this._colorDefault = value; }
        }

        public Color NumbersColor
        {
            get { return this._colorNumbers; }
            set { this._colorNumbers = value; }
        }

        public Color StringsColor
        {
            get { return this._colorStrings; }
            set { this._colorStrings = value; }
        }

        public Color KeyWordsColor
        {
            get { return this._colorKeyWords; }
            set { this._colorKeyWords = value; }
        }

        public Color IdentifiersColor
        {
            get { return this._colorIdentifiers; }
            set { this._colorIdentifiers = value; }
        }
    }

    class Token
    {
        private string _value = "";
        private TokenType _tokenType = TokenType.UNKNOWN;
        private int _line = 0;
        private int _index = 0;
        private int _lineIndex = 0;
        private Color _color = Color.Black;

        public Token()
        {
        }

        public Token(string value, TokenType tokenType, int line, int index)
        {
            this._value = value;
            this._tokenType = tokenType;
            this._index = index;
            this._line = line;
        }

        public Token(string value, TokenType tokenType, int index)
        {
            this._value = value;
            this._tokenType = tokenType;
            this._index = index;
        }

        public string Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        public int Line
        {
            get { return this._line; }
            set { this._line = value; }
        }

        public int Index
        {
            get { return this._index; }
            set { this._index = value; }
        }

        public int LineIndex
        {
            get { return this._lineIndex; }
            set { this._lineIndex = value; }
        }

        public TokenType TokenType
        {
            get { return this._tokenType; }
            set { this._tokenType = value; }
        }

        public Color Color
        {
            get { return this._color; }
            set { this._color = value; }
        }
    }
}
