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
            _defaultFgColor = fgColor;
            _lines = new List<TextLine>(100);
        }

        public int Count
        {
            get { return _lines.Count; }
        }

        public void Clear()
        {
            _lines.Clear();
        }

        public void ClearFormating()
        {
            foreach (TextLine tl in _lines)
                tl.RemoveFormating();
        }

        public void DeleteLine(int line)
        {
            _lines.RemoveAt(line);
        }

        public TextLine GetLine(int line)
        {
            return _lines[line];
        }

        public TextLine InsertLine(int index)
        {
            TextLine tl = new TextLine(_defaultFgColor);
            _lines.Insert(index, tl);
            return tl;
        }

        public Color ForeColor
        {
            get { return _defaultFgColor; }
            set 
            { 
                _defaultFgColor = value;
                foreach (TextLine tl in _lines)
                {
                    tl.ForeColor = value;
                }
            }
        }
    }
}
