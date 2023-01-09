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
            _regex = new Regex(_regexExpression, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public List<Token> GetTokens(string text)
        {
            List<Token> ret = new List<Token>();
            string[] lines = text.Replace("\r", "").Split(new char[] {'\n'});
            int lineIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                MatchCollection matches = _regex.Matches(lines[i]);

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

            GetTokenTypes(ref ret);

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
                    token.Color = _colorKeyWords;
                }
                else if (token.Value[0] == '"' && token.Value[token.Value.Length - 1] == '"')
                {
                    token.TokenType = TokenType.LITERAL_STRING;
                    token.Color = _colorStrings;
                }
                else if (token.Value[0] == '\'' && token.Value[token.Value.Length - 1] == '\'')
                {
                    token.TokenType = TokenType.LITERAL_STRING;
                    token.Color = _colorStrings;
                }
                else if (token.Value == "-" || token.Value == "+" || token.Value == "+=" || token.Value == "-=")
                {
                    token.TokenType = TokenType.MATH;
                    token.Color = _colorDefault;
                }
                else if (token.Value == "(")
                {
                    token.TokenType = TokenType.PARENTHESIS_LEFT;
                    token.Color = _colorDefault;
                }
                else if (token.Value == ")")
                {
                    token.TokenType = TokenType.PARENTHESIS_RIGHT;
                    token.Color = _colorDefault;
                }
                else if (token.Value == "[")
                {
                    token.TokenType = TokenType.BRACKET_LEFT;
                    token.Color = _colorDefault;
                }
                else if (token.Value == "]")
                {
                    token.TokenType = TokenType.BRACKET_RIGHT;
                    token.Color = _colorDefault;
                }
                else if (int.TryParse(token.Value, out iTest))
                {
                    token.TokenType = TokenType.LITERAL_INT;
                    token.Color = _colorNumbers;
                }
                else if (float.TryParse(token.Value, out fTest))
                {
                    token.TokenType = TokenType.LITERNAL_NUMBERIC;
                    token.Color = _colorNumbers;
                }
                else
                {
                    token.TokenType = TokenType.IDENTIFIER;
                    token.Color = _colorIdentifiers;
                }
            }
        }

        protected virtual bool IsKeyWord(string identifier)
        {
            return false;
        }

        public Color DefaultColor
        {
            get { return _colorDefault; }
            set { _colorDefault = value; }
        }

        public Color NumbersColor
        {
            get { return _colorNumbers; }
            set { _colorNumbers = value; }
        }

        public Color StringsColor
        {
            get { return _colorStrings; }
            set { _colorStrings = value; }
        }

        public Color KeyWordsColor
        {
            get { return _colorKeyWords; }
            set { _colorKeyWords = value; }
        }

        public Color IdentifiersColor
        {
            get { return _colorIdentifiers; }
            set { _colorIdentifiers = value; }
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
            _value = value;
            _tokenType = tokenType;
            _index = index;
            _line = line;
        }

        public Token(string value, TokenType tokenType, int index)
        {
            _value = value;
            _tokenType = tokenType;
            _index = index;
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public int LineIndex
        {
            get { return _lineIndex; }
            set { _lineIndex = value; }
        }

        public TokenType TokenType
        {
            get { return _tokenType; }
            set { _tokenType = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}
