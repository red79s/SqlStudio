namespace FormatTextControl
{
    public class UndoRecord
    {
        public TextPos Start {  get; set; }
        public TextPos End { get; set; }
        public string Text { get; set; }
        public bool IsInsert { get; set; }

        public UndoRecord(TextPos start, TextPos end, string text, bool isInsert)
        {
            Start = start;
            End = end;
            Text = text;
            IsInsert = isInsert;
        }
    }
}
