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
            this._index = index;
            this._text = text;
            this._fgColor = fgColor;
            this._bgColorSet = false;
        }

        public TextLineSegment(int index, string text, Color fgColor, Color bgColor)
        {
            this._index = index;
            this._text = text;
            this._fgColor = fgColor;
            this._bgColor = bgColor;
            this._bgColorSet = true;
        }

        public int Index
        {
            get { return this._index; }
            set { this._index = value; }
        }

        public Color FgColor
        {
            get { return this._fgColor; }
            set { this._fgColor = value; }
        }

        public Color BgColor
        {
            get { return this._bgColor; }
            set
            {
                this._bgColor = value;
                this._bgColorSet = true;
            }
        }

        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

        public bool BgColorSet
        {
            get { return this._bgColorSet; }
            set { this._bgColorSet = value; }
        }
    }
}
