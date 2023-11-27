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
            _sbLine = new StringBuilder();
            _defaultFgColor = fgColor;
            _formating = new List<TextLineFormating>();
        }

        public TextLine(string text, Color fgColor)
        {
            _sbLine = new StringBuilder(text);
            _defaultFgColor = fgColor;
            _formating = new List<TextLineFormating>();
        }

        public Color ForeColor
        {
            get { return _defaultFgColor; }
            set
            {
                _defaultFgColor = value;
            }
        }

        public void InsertText(int index, string text)
        {
            _sbLine.Insert(index, text);
        }

        public void InsertText(int index, string text, Color fgColor)
        {
            InsertText(index, text);
            AppendFormating(index, text.Length, fgColor);
        }

        public void InsertText(int index, string text, Color fgColor, Color bgColor)
        {
            InsertText(index, text);
            AppendFormating(index, text.Length, fgColor, bgColor);
        }

        public void AppendText(string text)
        {
            _sbLine.Append(text);
        }

        public void AppendText(TextLine textLine)
        {
            int index = Length;
            AppendText(textLine.GetText());
            List<TextLineFormating> formating = textLine.GetFormating();
            foreach (TextLineFormating format in formating)
            {
                if (format.BgColorSet)
                    AppendFormating(index + format.Index, format.Length, format.FgColor, format.BgColor);
                else
                    AppendFormating(index + format.Index, format.Length, format.FgColor);
            }
        }

        public void AppendText(string text, Color fgColor)
        {
            AppendText(text);
            AppendFormating(0, text.Length, fgColor);
        }

        public void AppendText(string text, Color fgColor, Color bgColor)
        {
            AppendText(text);
            AppendFormating(0, text.Length, fgColor, bgColor);
        }

        public void RemoveText()
        {
            _sbLine.Remove(0, _sbLine.Length);
            RemoveFormating();
        }

        public void RemoveText(int index, int length)
        {
            _sbLine.Remove(index, length);
            RemoveFormating(index, length);
        }

        public void AppendFormating(int index, int length, Color fgColor)
        {
            AppendFormating(index, length);
            _formating.Add(new TextLineFormating(index, length, fgColor));
        }

        public void AppendFormating(int index, int length, Color fgColor, Color bgColor)
        {
            AppendFormating(index, length);
            _formating.Add(new TextLineFormating(index, length, fgColor, bgColor));
        }

        private void AppendFormating(int index, int length)
        {
            _formatingSorted = false;

            for (int i = _formating.Count - 1; i >= 0; i--)
            {
                TextLineFormating format = _formating[i];
           
                if (index <= format.Index)
                    format.Index += length;
                else if (index < (format.Index + format.Length))
                {
                    int diff = index - format.Index;
                    TextLineFormating nFormat = new TextLineFormating(index + length, format.Length - diff, format.FgColor);
                    if (format.BgColorSet)
                        nFormat.BgColor = format.BgColor;
                    _formating.Add(nFormat);
                    format.Length = diff;
                }
            }
        }

        public void RemoveFormating()
        {
            _formatingSorted = false;
            _formating.Clear();
        }

        public void RemoveFormating(int index, int length)
        {
           _formatingSorted = false;

            for (int i = _formating.Count - 1; i >= 0; i--)
            {
                TextLineFormating format = _formating[i];
                if (format.Index >= index && (format.Index + format.Length) <= (index + length)) //inside, remove
                    _formating.RemoveAt(i);
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
            return new List<TextLineFormating>(_formating);
        }

        public float LineWidth
        {
            get { return _lineWidth; }
            set { _lineWidth = value; }
        }

        public int Length
        {
            get { return _sbLine.Length; }
        }

        public string GetText()
        {
            return _sbLine.ToString();
        }

        public string GetText(int index, int length)
        {
            return _sbLine.ToString(index, length);
        }

        public List<TextLineSegment> GetFormatedText()
        {
            return GenerateTextSegments(0, _sbLine.Length);
        }

        public List<TextLineSegment> GetFormatedText(int index, int length)
        {
            return GenerateTextSegments(index, length);
        }

        #region IEnumerable<TextLineSegment> Members

        IEnumerator<TextLineSegment> IEnumerable<TextLineSegment>.GetEnumerator()
        {
            _textLineSegments = GenerateTextSegments(0, _sbLine.Length);
            return _textLineSegments.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _textLineSegments = GenerateTextSegments(0, _sbLine.Length);
            return _textLineSegments.GetEnumerator();
        }

        #endregion

        private void SortFormating()
        {
            if (_formatingSorted)
                return;
            _formating.Sort(delegate(TextLineFormating t1, TextLineFormating t2) { return t1.Index.CompareTo(t2.Index); });
            _formatingSorted = true;
        }

        private List<TextLineSegment> GenerateTextSegments(int index, int length)
        {
            List<TextLineSegment> ret = new List<TextLineSegment>();
            if (_formating.Count < 1)
            {
                ret.Add(new TextLineSegment(index, GetText(index, length), _defaultFgColor));
            }
            else
            {
                SortFormating();

                int lastIndex = index;
                foreach (TextLineFormating format in _formating)
                {
                    //if ((format.Index + format.Length) > ._sbLine.Length)
                    //    format.Length = ._sbLine.Length - format.Index;

                    if ((format.Index + format.Length) < index)
                        continue;
                    if (format.Index >= (index + length))
                        continue;

                    if (lastIndex < format.Index)
                    {
                        ret.Add(new TextLineSegment(lastIndex, GetText(lastIndex, format.Index - lastIndex), _defaultFgColor));
                        lastIndex = format.Index;
                    }
                    TextLineSegment tls = new TextLineSegment(format.Index, GetText(format.Index, format.Length), format.FgColor);
                    if (format.BgColorSet)
                        tls.BgColor = format.BgColor;
                    ret.Add(tls);
                    lastIndex = format.Index + format.Length;
                }
                if (lastIndex < (index + length))
                    ret.Add(new TextLineSegment(lastIndex, GetText(lastIndex, (index + length) - lastIndex), _defaultFgColor));
            }

            return ret;
        }
    }
}
