using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FormatTextControl
{
    class StringDraw
    {
        private Graphics _g = null;

        public StringDraw(Graphics g)
        {
            this._g = g;
            this._g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        public SizeF MeasureString(string str, Font font)
        {
            StringFormat format = StringFormat.GenericTypographic;
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges ={ new CharacterRange(0, str.Length) };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);
            format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            format.SetTabStops(0, _tabStops);
            regions = this._g.MeasureCharacterRanges(str, font, rect, format);
            rect = regions[0].GetBounds(this._g);

            return new SizeF(rect.Width, rect.Height);
        }

        private float[] _tabStops = { 50, 50, 50, 50, 50 };

        public void DrawString(string str, Font font, Color textColor, float x, float y)
        {
            StringFormat format = StringFormat.GenericTypographic;
            format.SetTabStops(0, _tabStops);
            format.FormatFlags = StringFormatFlags.FitBlackBox;
            this._g.DrawString(str, font, new SolidBrush(textColor), x, y, format);
        }

        public void DrawString(char c, Font font, SolidBrush textBrush, float x, float y)
        {
            StringFormat format = StringFormat.GenericTypographic;
            format.SetTabStops(0, _tabStops);
            format.FormatFlags = StringFormatFlags.FitBlackBox;
            this._g.DrawString(c.ToString(), font, textBrush, x, y, format);
        }
    }
}
