using System.Drawing;

namespace CommandPrompt
{
    /// <summary>Colours a FormatTextControl holding a SQL script. Colours follow the SSMS defaults.</summary>
    public class SqlScriptHighlighter
    {
        public Color KeywordColor { get; set; } = Color.Blue;
        public Color DataTypeColor { get; set; } = Color.Blue;
        public Color FunctionColor { get; set; } = Color.FromArgb(255, 0, 255);
        public Color GlobalVariableColor { get; set; } = Color.FromArgb(255, 0, 255);
        public Color VariableColor { get; set; } = Color.FromArgb(0, 128, 128);
        public Color TempTableColor { get; set; } = Color.FromArgb(128, 64, 0);
        public Color StringColor { get; set; } = Color.Red;
        public Color NumberColor { get; set; } = Color.FromArgb(9, 134, 88);
        public Color CommentColor { get; set; } = Color.Green;
        public Color OperatorColor { get; set; } = Color.Gray;
        public Color BatchSeparatorColor { get; set; } = Color.Blue;
        public Color BatchSeparatorBackColor { get; set; } = Color.FromArgb(230, 230, 230);

        /// <summary>Re-colours every line. Cheap enough to run after each edit for scripts of many thousand lines.</summary>
        public void Highlight(FormatTextControl.FormatTextControl control)
        {
            var state = SqlLexState.Normal;
            int lineCount = control.LineCount;
            for (int line = 0; line < lineCount; line++)
            {
                string text = control.GetText(line);
                control.RemoveFormating(line);
                foreach (var token in SqlTokenizer.TokenizeLine(text, ref state))
                {
                    if (token.Length == 0)
                        continue;

                    if (token.Type == SqlTokenType.BatchSeparator)
                    {
                        control.AddFormating(line, token.Index, token.Length, BatchSeparatorColor, BatchSeparatorBackColor);
                        continue;
                    }

                    Color? color = GetColor(token.Type);
                    if (color.HasValue)
                        control.AddFormating(line, token.Index, token.Length, color.Value);
                }
            }
        }

        private Color? GetColor(SqlTokenType type)
        {
            switch (type)
            {
                case SqlTokenType.Keyword: return KeywordColor;
                case SqlTokenType.DataType: return DataTypeColor;
                case SqlTokenType.Function: return FunctionColor;
                case SqlTokenType.GlobalVariable: return GlobalVariableColor;
                case SqlTokenType.Variable: return VariableColor;
                case SqlTokenType.TempTable: return TempTableColor;
                case SqlTokenType.String: return StringColor;
                case SqlTokenType.Number: return NumberColor;
                case SqlTokenType.Comment: return CommentColor;
                case SqlTokenType.Operator: return OperatorColor;
                default: return null;
            }
        }
    }
}
