using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FormatTextControl
{
    class TextLineSegment
    {
        private int _index = 0;
        private Color _fgColor = Color.Black;
        private Color _bgColor = Color.White;
        private bool _bgColorSet = false;
        private string _text = "";

        public TextLineSegment(int index, string text, Color fgColor)
        {
            _index = index;
            _text = text;
            _fgColor = fgColor;
            _bgColorSet = false;
        }

        public TextLineSegment(int index, string text, Color fgColor, Color bgColor)
        {
            _index = index;
            _text = text;
            _fgColor = fgColor;
            _bgColor = bgColor;
            _bgColorSet = true;
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public Color FgColor
        {
            get { return _fgColor; }
            set { _fgColor = value; }
        }

        public Color BgColor
        {
            get { return _bgColor; }
            set
            {
                _bgColor = value;
                _bgColorSet = true;
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool BgColorSet
        {
            get { return _bgColorSet; }
            set { _bgColorSet = value; }
        }
    }
}
