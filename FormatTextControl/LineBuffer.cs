using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FormatTextControl
{
    class LineBuffer
    {
        private List<TextLine> _lines = null;
        private Color _defaultFgColor = Color.Black;

        public LineBuffer(Color fgColor)
        {
            this._defaultFgColor = fgColor;
            this._lines = new List<TextLine>(100);
        }

        public int Count
        {
            get { return this._lines.Count; }
        }

        public void Clear()
        {
            this._lines.Clear();
        }

        public void ClearFormating()
        {
            foreach (TextLine tl in this._lines)
                tl.RemoveFormating();
        }

        public void DeleteLine(int line)
        {
            this._lines.RemoveAt(line);
        }

        public TextLine GetLine(int line)
        {
            return this._lines[line];
        }

        public TextLine InsertLine(int index)
        {
            TextLine tl = new TextLine(this._defaultFgColor);
            this._lines.Insert(index, tl);
            return tl;
        }

        public Color ForeColor
        {
            get { return this._defaultFgColor; }
            set 
            { 
                this._defaultFgColor = value;
                foreach (TextLine tl in this._lines)
                {
                    tl.ForeColor = value;
                }
            }
        }
    }
}
