using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FormatTextControl
{
    class TextLineFormating
    {
        private int _index = 0;
        private int _length = 0;
        private Color _fgColor = Color.Black;
        private Color _bgColor = Color.White;
        private bool _bgColorSet = false;

        public TextLineFormating(int index, int length, Color fgColor)
        {
            _index = index;
            _length = length;
            _fgColor = fgColor;
            _bgColorSet = false;
        }

        public TextLineFormating(int index, int length, Color fgColor, Color bgColor)
        {
            _index = index;
            _length = length;
            _fgColor = fgColor;
            _bgColor = bgColor;
            _bgColorSet = true;
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
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

        public bool BgColorSet
        {
            get { return _bgColorSet; }
            set { _bgColorSet = value; }
        }
    }
}
