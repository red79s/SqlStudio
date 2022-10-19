using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FormatTextControl
{
    class TextLine : IEnumerable<TextLineSegment>
    {
        private StringBuilder _sbLine = null;
        private float _lineWidth = 0;
        private Color _defaultFgColor = Color.Black;
        private List<TextLineFormating> _formating = null;
        private List<TextLineSegment> _textLineSegments = null;
        private bool _formatingSorted = false;

        public TextLine(Color fgColor)
        {
            this._sbLine = new StringBuilder();
            this._defaultFgColor = fgColor;
            this._formating = new List<TextLineFormating>();
        }

        public TextLine(string text, Color fgColor)
        {
            this._sbLine = new StringBuilder(text);
            this._defaultFgColor = fgColor;
            this._formating = new List<TextLineFormating>();
        }

        public Color ForeColor
        {
            get { return this._defaultFgColor; }
            set
            {
                this._defaultFgColor = value;
            }
        }

        public void InsertText(int index, string text)
        {
            this._sbLine.Insert(index, text);
        }

        public void InsertText(int index, string text, Color fgColor)
        {
            this.InsertText(index, text);
            this.AppendFormating(index, text.Length, fgColor);
        }

        public void InsertText(int index, string text, Color fgColor, Color bgColor)
        {
            this.InsertText(index, text);
            this.AppendFormating(index, text.Length, fgColor, bgColor);
        }

        public void AppendText(string text)
        {
            this._sbLine.Append(text);
        }

        public void AppendText(TextLine textLine)
        {
            int index = this.Length;
            this.AppendText(textLine.GetText());
            List<TextLineFormating> formating = textLine.GetFormating();
            foreach (TextLineFormating format in formating)
            {
                if (format.BgColorSet)
                    this.AppendFormating(index + format.Index, format.Length, format.FgColor, format.BgColor);
                else
                    this.AppendFormating(index + format.Index, format.Length, format.FgColor);
            }
        }

        public void AppendText(string text, Color fgColor)
        {
            this.AppendText(text);
            this.AppendFormating(0, text.Length, fgColor);
        }

        public void AppendText(string text, Color fgColor, Color bgColor)
        {
            this.AppendText(text);
            this.AppendFormating(0, text.Length, fgColor, bgColor);
        }

        public void RemoveText()
        {
            this._sbLine.Remove(0, this._sbLine.Length);
            this.RemoveFormating();
        }

        public void RemoveText(int index, int length)
        {
            this._sbLine.Remove(index, length);
            this.RemoveFormating(index, length);
        }

        public void AppendFormating(int index, int length, Color fgColor)
        {
            this.AppendFormating(index, length);
            this._formating.Add(new TextLineFormating(index, length, fgColor));
        }

        public void AppendFormating(int index, int length, Color fgColor, Color bgColor)
        {
            this.AppendFormating(index, length);
            this._formating.Add(new TextLineFormating(index, length, fgColor, bgColor));
        }

        private void AppendFormating(int index, int length)
        {
            this._formatingSorted = false;

            for (int i = this._formating.Count - 1; i >= 0; i--)
            {
                TextLineFormating format = this._formating[i];
           
                if (index <= format.Index)
                    format.Index += length;
                else if (index < (format.Index + format.Length))
                {
                    int diff = index - format.Index;
                    TextLineFormating nFormat = new TextLineFormating(index + length, format.Length - diff, format.FgColor);
                    if (format.BgColorSet)
                        nFormat.BgColor = format.BgColor;
                    this._formating.Add(nFormat);
                    format.Length = diff;
                }
            }
        }

        public void RemoveFormating()
        {
            this._formatingSorted = false;
            this._formating.Clear();
        }

        public void RemoveFormating(int index, int length)
        {
           this._formatingSorted = false;

            for (int i = this._formating.Count - 1; i >= 0; i--)
            {
                TextLineFormating format = this._formating[i];
                if (format.Index >= index && (format.Index + format.Length) <= (index + length)) //inside, remove
                    this._formating.RemoveAt(i);
                else if (format.Index >= index && format.Index < (index + length)) // overlap to left, shrink
                {
                    format.Length -= ((index + length) - format.Index);
                }
                else if ((format.Index + format.Length - 1) >= index && (format.Index + format.Length - 1) < (index + length)) //overlap to right, shrink
                {
                    format.Length -= ((format.Length + format.Index) - index);
                }
                else if (format.Index <= index && (format.Index + format.Length) >= (index + length)) //formating encloses removed chars
                {
                    format.Length -= length;
                }
                else if (format.Index >= (index + length))
                {
                    format.Index -= length;
                }
            }
        }

        public List<TextLineFormating> GetFormating()
        {
            return new List<TextLineFormating>(this._formating);
        }

        public float LineWidth
        {
            get { return this._lineWidth; }
            set { this._lineWidth = value; }
        }

        public int Length
        {
            get { return this._sbLine.Length; }
        }

        public string GetText()
        {
            return this._sbLine.ToString();
        }

        public string GetText(int index, int length)
        {
            return this._sbLine.ToString(index, length);
        }

        public List<TextLineSegment> GetFormatedText()
        {
            return this.GenerateTextSegments(0, this._sbLine.Length);
        }

        public List<TextLineSegment> GetFormatedText(int index, int length)
        {
            return this.GenerateTextSegments(index, length);
        }

        #region IEnumerable<TextLineSegment> Members

        IEnumerator<TextLineSegment> IEnumerable<TextLineSegment>.GetEnumerator()
        {
            this._textLineSegments = this.GenerateTextSegments(0, this._sbLine.Length);
            return this._textLineSegments.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this._textLineSegments = this.GenerateTextSegments(0, this._sbLine.Length);
            return this._textLineSegments.GetEnumerator();
        }

        #endregion

        private void SortFormating()
        {
            if (this._formatingSorted)
                return;
            this._formating.Sort(delegate(TextLineFormating t1, TextLineFormating t2) { return t1.Index.CompareTo(t2.Index); });
            this._formatingSorted = true;
        }

        private List<TextLineSegment> GenerateTextSegments(int index, int length)
        {
            List<TextLineSegment> ret = new List<TextLineSegment>();
            if (this._formating.Count < 1)
            {
                ret.Add(new TextLineSegment(index, this.GetText(index, length), this._defaultFgColor));
            }
            else
            {
                this.SortFormating();

                int lastIndex = index;
                foreach (TextLineFormating format in this._formating)
                {
                    //if ((format.Index + format.Length) > this._sbLine.Length)
                    //    format.Length = this._sbLine.Length - format.Index;

                    if ((format.Index + format.Length) < index)
                        continue;
                    if (format.Index >= (index + length))
                        continue;

                    if (lastIndex < format.Index)
                    {
                        ret.Add(new TextLineSegment(lastIndex, this.GetText(lastIndex, format.Index - lastIndex), this._defaultFgColor));
                        lastIndex = format.Index;
                    }
                    TextLineSegment tls = new TextLineSegment(format.Index, this.GetText(format.Index, format.Length), format.FgColor);
                    if (format.BgColorSet)
                        tls.BgColor = format.BgColor;
                    ret.Add(tls);
                    lastIndex = format.Index + format.Length;
                }
                if (lastIndex < (index + length))
                    ret.Add(new TextLineSegment(lastIndex, this.GetText(lastIndex, (index + length) - lastIndex), this._defaultFgColor));
            }

            return ret;
        }
    }
}
