using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace FormatTextControl
{
    public class FormatTextControl : ScrollableControl
    {
        public delegate void TextInsertedDelegate(object sender, TextPos start, TextPos end, string text);
        public event TextInsertedDelegate TextInserted;
        public delegate void IconLineClickedDelegate(object sender, int line);
        public event IconLineClickedDelegate IconLineClicked;
        public delegate void IconLineDoubleClickedDelegate(object sender, int line);
        public event IconLineDoubleClickedDelegate IconLineDoubleClicked;
        
        private LineBuffer _lineBuffer = null;
        private TextCaret _textCaret = null;
        private TextPos _caretPos = new TextPos(0, 0);
        
        private TextPos _selectionStart = new TextPos(0, 0);
        private TextPos _selectionEnd = new TextPos(0, 0);
        private TextPos _selectionAnchor = new TextPos(0, 0);
        private TextPos _selectionLast = new TextPos(0, 0);
        private bool _bSelectionInProgress = false;
        private IUndoHistoryManager _undoHistory;

        private float[] _charWidth = null;
        private int _lineHeight = 10;
        private int _charSpacing = 0;
        private int _lineIndent = 5;
        private int _lineSpacing = 1;
        private bool _bShowLineNumbers = true;
        private int _lineNumbersWidth = 30;
        private bool _bShowIconField = true;
        private int _iconFieldWidth = 20;
        private int _maxLineWidth = 1000;
        private bool _treatTabAsSpaces = true;
        private int _numSpacesForTab = 5;

        private bool _inUpdate = false;
        private bool _acceptsKeyInput = true;

        private SolidBrush _sbForeColor = null;
        private SolidBrush _sbBackColor = null;
        private SolidBrush _sbLineBackColor = null;
        private SolidBrush _sbLineForeColor = null;
        private SolidBrush _sbIconBackColor = null;
        private SolidBrush _sbBackColorSelection = null;

        public FormatTextControl()
        {
            _undoHistory = new UndoHistoryManager();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            _lineHeight = (int)base.Font.GetHeight() + _lineSpacing;
            _lineBuffer = new LineBuffer(ForeColor);
            _lineBuffer.InsertLine(0);
            _textCaret = new TextCaret(this);
            _textCaret.Size = new Size(1, _lineHeight);

            BackColor = Color.White;
            ForeColor = Color.Black;
            ForeColorLineNum = Color.FromArgb(64, 64, 64);
            BackColorLineNum = Color.FromArgb(224, 224, 224);
            BackColorIconPane = Color.Yellow;
            BackColorSelection = Color.FromArgb(122, 150, 223); 

            SetAutoScrollMinSize();

            Cursor = Cursors.IBeam;

            ContextMenuStrip = new ContextMenuStrip();
            //ContextMenuStrip.Popup += new EventHandler(ContextMenu_Popup);
            var miCut = new ToolStripMenuItem("Cut");
            miCut.Click += new EventHandler(miCut_Click);
            ContextMenuStrip.Items.Add(miCut);

            var miCopy = new ToolStripMenuItem("Copy");
            miCopy.Click += new EventHandler(miCopy_Click);
            ContextMenuStrip.Items.Add(miCopy);

            var miPaste = new ToolStripMenuItem("Paste");
            miPaste.Click += new EventHandler(miPaste_Click);
            ContextMenuStrip.Items.Add(miPaste);
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            if (SelectionStart < SelectionEnd)
            {
                ContextMenuStrip.Items[0].Enabled = true;
                ContextMenuStrip.Items[1].Enabled = true;
            }
            else
            {
                ContextMenuStrip.Items[0].Enabled = false;
                ContextMenuStrip.Items[1].Enabled = false;
            }

            if (Clipboard.ContainsText())
            {
                ContextMenuStrip.Items[2].Enabled = true;
            }
            else
            {
                ContextMenuStrip.Items[2].Enabled = false;
            }
        }

        void miPaste_Click(object sender, EventArgs e)
        {
            OnPaste();
        }

        void miCopy_Click(object sender, EventArgs e)
        {
            OnCopy();
        }

        void miCut_Click(object sender, EventArgs e)
        {
            OnCut();
        }

        public void Cut()
        {
            OnCut();
        }

        public void Copy()
        {
            OnCopy();
        }

        public void Paste()
        {
            OnPaste();
        }

        public void ClearUndoHistory()
        {
            _undoHistory.ClearUndoHistory();
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                _lineHeight = (int)base.Font.GetHeight() + _lineSpacing;
                _textCaret.Size = new Size(1, _lineHeight);
                _charWidth = null;
                SetAutoScrollMinSize();
                Invalidate();
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                _sbBackColor = new SolidBrush(BackColor);
                Invalidate();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                _sbForeColor = new SolidBrush(ForeColor);
                _lineBuffer.ForeColor = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ForeColorLineNum
        {
            get { return _sbLineForeColor.Color; }
            set 
            { 
                _sbLineForeColor = new SolidBrush(value);
                if (_bShowLineNumbers)
                    Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackColorLineNum
        {
            get { return _sbLineBackColor.Color; }
            set
            {
                _sbLineBackColor = new SolidBrush(value);
                if (_bShowLineNumbers)
                    Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackColorSelection
        {
            get { return _sbBackColorSelection.Color; }
            set 
            {
                _sbBackColorSelection = new SolidBrush(value);
                if (IsTextSelected())
                    Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowLineNumbers
        {
            get { return _bShowLineNumbers; }
            set
            {
                if (_bShowLineNumbers != value)
                {
                    _bShowLineNumbers = value;
                    PositionCaret();
                    Invalidate();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color BackColorIconPane
        {
            get { return _sbIconBackColor.Color; }
            set 
            { 
                _sbIconBackColor = new SolidBrush(value);
                if (_bShowIconField)
                    Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowIconPane
        {
            get { return _bShowIconField; }
            set
            {
                if (_bShowIconField != value)
                {
                    _bShowIconField = value;
                    PositionCaret();
                    Invalidate();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AcceptsKeyInput
        {
            get { return _acceptsKeyInput; }
            set { _acceptsKeyInput = value; }
        }

        public override string Text
        {
            get
            {
                StringBuilder sbRet = new StringBuilder();
                for (int i = 0; i < _lineBuffer.Count; i++)
                {
                    sbRet.Append(_lineBuffer.GetLine(i).GetText());
                }
                return sbRet.ToString();
            }
            set
            {
                _lineBuffer.Clear();
                string[] lines = value.Replace("\r", "").Split(new char[] {'\n'});
                for (int i = 0; i < lines.Length; i++)
                {
                    _lineBuffer.InsertLine(i).InsertText(0, lines[i]);
                }

                if (lines.Length < 1)
                    _lineBuffer.InsertLine(0);

                CaretPos = new TextPos(0, 0);
                AutoScrollPosition = new Point(0, 0);
                MeasureLineWidthAll();
                InvalidateFromLine(0);
            }
        }

        public string[] Lines
        {
            get
            {
                string[] ret = new string[_lineBuffer.Count];
                for (int i = 0; i < _lineBuffer.Count; i++)
                    ret[i] = _lineBuffer.GetLine(i).GetText();
                return ret;
            }
        }

        public void Clear()
        {
            _lineBuffer.Clear();
            _lineBuffer.InsertLine(0);
            CaretPos = new TextPos(0, 0);
            AutoScrollPosition = new Point(0, 0);
            MeasureLineWidthAll();
            InvalidateFromLine(0);
        }

        public void BeginUpdate()
        {
            _inUpdate = true;
        }

        public void EndUpdate()
        {
            _inUpdate = false;
            Invalidate();
        }

        public void SetText(int line, string text)
        {
            TextLine tl = _lineBuffer.GetLine(line);
            tl.RemoveText();
            tl.AppendText(text);
            InvalidateLine(line);
        }

        public void InsertLine(int line, string text)
        {
            _lineBuffer.InsertLine(line).InsertText(0, text);
            InvalidateFromLine(line);
            SetAutoScrollMinSize();
        }

        public void RemoveLine(int line)
        {
            _lineBuffer.DeleteLine(line);
            InvalidateFromLine(line);
            SetAutoScrollMinSize();
        }

        public void AppendText(string text)
        {
            TextPos pos = new TextPos(_lineBuffer.Count - 1, _lineBuffer.GetLine(_lineBuffer.Count - 1).Length);
            InsertTextAtPos(pos, text, true);
        }

        public void InsertTextAtCaret(string text)
        {
            InsertTextAtPos(CaretPos, text, true);
        }

        public string GetText(int line)
        {
            return _lineBuffer.GetLine(line).GetText();
        }

        public string GetText(int line, int index, int length)
        {
            return _lineBuffer.GetLine(line).GetText(index, length);
        }

        public string GetText(TextPos start, TextPos end)
        {
            if (start >= end)
                return "";

            if (start.Line == end.Line)
            {
                return _lineBuffer.GetLine(start.Line).GetText(start.Index, end.Index - start.Index);
            }

            StringBuilder sbRet = new StringBuilder();
            for (int i = start.Line; i <= end.Line; i++)
            {
                TextLine tl = _lineBuffer.GetLine(i);
                if (i == start.Line)
                    sbRet.Append(tl.GetText(start.Index, tl.Length - start.Index) + Environment.NewLine);
                else if (i == end.Line)
                    sbRet.Append(tl.GetText(0, end.Index));
                else
                    sbRet.Append(tl.GetText() + Environment.NewLine);
            }
            return sbRet.ToString();
        }

        public int LineCount
        {
            get { return _lineBuffer.Count; }
        }

        public int GetLineLength(int line)
        {
            return _lineBuffer.GetLine(line).Length;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPos CaretPos
        {
            get { return _caretPos; }
            set
            {
                if (OnBeforeCaretMove(_caretPos, value))
                {
                    _caretPos = value;
                    if (_caretPos.Line < 0 || _caretPos.Line >= _lineBuffer.Count)
                        throw new ArgumentOutOfRangeException("Invalid line number");
                    if (_caretPos.Index < 0 || _caretPos.Index > _lineBuffer.GetLine(_caretPos.Line).Length)
                        throw new ArgumentOutOfRangeException("Invadlid index on line");
                    PositionCaret();
                }
            }
        }

        public void ScrollToCaret()
        {
            int line = CaretPos.Line;
            int y = line * _lineHeight;
            AutoScrollPosition = new Point(-AutoScrollPosition.X, y);
        }

        protected virtual bool OnBeforeCaretMove(TextPos oldPos, TextPos newPos)
        {
            return true;
        }

        public TextPos GetTextEnd()
        {
            return new TextPos(_lineBuffer.Count - 1, _lineBuffer.GetLine(_lineBuffer.Count - 1).Length);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPos SelectionStart
        {
            get { return _selectionStart; }
            set { _selectionStart = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPos SelectionEnd
        {
            get { return _selectionEnd; }
            set { _selectionEnd = value; }
        }

        public void AddFormating(int line, int index, int length, Color textColor)
        {
            _lineBuffer.GetLine(line).AppendFormating(index, length, textColor);
        }

        public void AddFormating(int line, int index, int length, Color textColor, Color bgColor)
        {
            _lineBuffer.GetLine(line).AppendFormating(index, length, textColor, bgColor);
        }

        public void RemoveFormating(int line)
        {
            _lineBuffer.GetLine(line).RemoveFormating();
        }

        public void RemoveFormating(int line, int index, int length)
        {
            _lineBuffer.GetLine(line).RemoveFormating(index, length);
        }

        private void UpdateLineNumWidth()
        {
            if (!_bShowLineNumbers || _charWidth == null)
                return;

            int max = _lineBuffer.Count - 1;
            if (max < 99)
                max = 99;

            string s = max.ToString();
            float width = 0;
            for (int i = 0; i < s.Length; i++)
            {
                width += GetCharWidth((int)'0');
            }
            width += 5;

            int iWidth = (int)width;
            if (_lineNumbersWidth != iWidth)
            {
                _lineNumbersWidth = iWidth;
                Invalidate();
            }
        }

        private void SetAutoScrollMinSize()
        {
            AutoScrollMinSize = new Size(_maxLineWidth + 10, (_lineBuffer.Count * _lineHeight) + 10);
        }

        public void ScrollLines(int lines)
        {
            AutoScrollPosition = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y + (lines * _lineHeight));
        }

        private bool IsCaretInsideScreen()
        {
            if (_textCaret.Position.Y < 0 || _textCaret.Position.Y > (ClientRectangle.Bottom - (_lineHeight + 1)))
                return false;
            if (_textCaret.Position.X < 1 || _textCaret.Position.X > (ClientRectangle.Right - 1))
                return false;
            return true;
        }

        private bool IsCordsInsideTextArea(Point p)
        {
            int x = AutoScrollPosition.X;
            if (_bShowLineNumbers)
                x += _lineNumbersWidth;
            if (_bShowIconField)
                x += _iconFieldWidth;

            if (p.X > x)
                return true;

            return false;
        }

        private bool IsCordsInsideLineNumArea(Point p)
        {
            if (!_bShowLineNumbers)
                return false;
            if (p.X < (_lineNumbersWidth + AutoScrollPosition.X))
                return true;
            return false;
        }

        private bool IsCordsInsideIconArea(Point p)
        {
            if (!_bShowIconField)
                return false;
            int xMin = AutoScrollPosition.X;
            if (_bShowLineNumbers)
                xMin += _lineNumbersWidth;
            int xMax = xMin + _iconFieldWidth;
            if (p.X >= xMin && p.X < xMax)
                return true;
            return false;
        }

        private int CordsToLine(int y)
        {
            int line = (y - AutoScrollPosition.Y) / _lineHeight;
            if (line >= _lineBuffer.Count)
                line = _lineBuffer.Count - 1;
            if (line < 0)
                line = 0;
            return line;
        }

        private TextPos CordsToTextPos(Point p)
        {
            int line = CordsToLine(p.Y);
            int index = 0;

            if (line >= _lineBuffer.Count)
            {
                return new TextPos(0, index);
            }
            else if (line < 0)
            {
                return new TextPos(0, index);
            }

            string s = _lineBuffer.GetLine(line).GetText();
            float xOffset = _lineIndent + AutoScrollPosition.X;
            if (_bShowLineNumbers)
                xOffset += _lineNumbersWidth;
            if (_bShowIconField)
                xOffset += _iconFieldWidth;

            for (int i = 0; i < s.Length; i++)
            {
                float width = GetCharWidth((int)s[i]);
                if (p.X < (xOffset + (width / 2)))
                {
                    return new TextPos(line, i);
                }
                xOffset += width + _charSpacing;
            }

            return new TextPos(line, s.Length);
        }

        private void InvalidateSelection()
        {
            if (_inUpdate)
                return;

            for (int i = SelectionStart.Line; i <= SelectionEnd.Line; i++ )
            {
                InvalidateLine(i);
            }
        }

        private void InvalidateFromLine(int line)
        {
            if (_inUpdate)
                return;

            int beginY = (line * _lineHeight) + AutoScrollPosition.Y;
            Invalidate(new Rectangle(0, beginY, ClientRectangle.Width, ClientRectangle.Height - beginY));
        }

        public void InvalidateLine(int line)
        {
            if (_inUpdate)
                return;

            int y = (line * _lineHeight) + AutoScrollPosition.Y;
            int x = AutoScrollPosition.X;
            if (_bShowLineNumbers)
                x += _lineNumbersWidth;
            if (_bShowIconField)
                x += _iconFieldWidth;

            Invalidate(new Rectangle(x, y, _maxLineWidth - x, _lineHeight));
        }

        private bool IsTextPosInsideSelection(int line, int index)
        {
            if (SelectionStart == SelectionEnd)
                return false;
            if (line < SelectionStart.Line || line > SelectionEnd.Line)
                return false;

            if (line > SelectionStart.Line && line < SelectionEnd.Line)
                return true;

            if (line == SelectionStart.Line && index < SelectionStart.Index)
                return false;
            if (line == SelectionEnd.Line && index >= SelectionEnd.Index)
                return false;

            return true;
        }

        private int FindLefWordBoundary(int line, int index)
        {
            string s = _lineBuffer.GetLine(line).GetText();
            for (int i = index - 1; i >= 0; i--)
            {
                if (CharIsSeperatorChar(s[i]))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private int FindRightWordBoundary(int line, int index)
        {
            string s = _lineBuffer.GetLine(line).GetText();
            for (int i = index; i < s.Length; i++)
            {
                if (CharIsSeperatorChar(s[i]))
                {
                    return i;
                }
            }
            return s.Length;
        }

        private bool CharIsSeperatorChar(char c)
        {
            switch (c)
            {
                case ' ':
                case '.':
                case '(':
                case ')':
                case '[':
                case ']':
                case ',':
                case ';':
                case '\'':
                    return true;
                default:
                    return false;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCordsInsideTextArea(e.Location))
                {
                    if (TrySelectionInsideString(CaretPos.Line, CaretPos.Index))
                    {
                        _bSelectionInProgress = false;
                        return;
                    }

                    int left = FindLefWordBoundary(CaretPos.Line, CaretPos.Index);
                    int right = FindRightWordBoundary(CaretPos.Line, CaretPos.Index);
                    if (left < right)
                    {
                        SelectionStart = new TextPos(CaretPos.Line, left);
                        SelectionEnd = new TextPos(CaretPos.Line, right);
                        CaretPos = SelectionEnd;
                        InvalidateLine(CaretPos.Line);
                    }
                    else
                    {
                        SelectionStart = _caretPos;
                        SelectionEnd = _caretPos;
                    }
                    _bSelectionInProgress = false;
                }
                else if (IsCordsInsideIconArea(e.Location))
                {
                    if (IconLineDoubleClicked != null)
                        IconLineDoubleClicked(this, CordsToLine(e.Y));
                }
            }
        }

        bool TrySelectionInsideString(int line, int index)
        {
            var text = GetText(line);
            char stringConstant = (char)0;
            bool isInsideString = false;
            bool prevCharIsExcape = false;
            int stringBegin = 0;
          
            for (int i = 0; i < text.Length; i++)
            {
                if (isInsideString && text[i] == stringConstant && !prevCharIsExcape)
                {
                    isInsideString = false;

                    if (index >= stringBegin && index <= i)
                    {
                        SelectionStart = new TextPos(CaretPos.Line, stringBegin + 1);
                        SelectionEnd = new TextPos(CaretPos.Line, i);
                        CaretPos = SelectionEnd;
                        InvalidateLine(CaretPos.Line);
                        return true;
                    }
                }
                else if (!prevCharIsExcape && (text[i] == '"' || text[i] == '\''))
                {
                    stringBegin = i;
                    stringConstant = text[i];
                    isInsideString = true;
                }

                if (text[i] == '\\' && !prevCharIsExcape)
                    prevCharIsExcape = true;
            }

            return false;
        }
        
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCordsInsideIconArea(e.Location))
                {
                    if (IconLineClicked != null)
                        IconLineClicked(this, CordsToLine(e.Y));
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused)
                Focus();

            if (e.Button == MouseButtons.Left)
            {
                if (IsCordsInsideTextArea(e.Location))
                {
                    _bSelectionInProgress = true;
                    InvalidateSelection();
                    _selectionAnchor = CordsToTextPos(e.Location);
                    _selectionLast = _selectionAnchor;
                    SelectionStart = _selectionAnchor;
                    SelectionEnd = _selectionAnchor;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_bSelectionInProgress)
                {
                    if (e.Y > ClientRectangle.Height)
                    {
                        int diff = e.Y - ClientRectangle.Height;
                        AutoScrollPosition = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y + diff);
                    }
                    else if (e.Y < 0)
                    {
                        AutoScrollPosition = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y + e.Y);
                    }

                    TextPos p = CordsToTextPos(e.Location);

                    if (p == _selectionLast)
                        return;

                    if (p > _selectionAnchor)
                    {
                        SelectionStart = _selectionAnchor;
                        SelectionEnd = p;
                    }
                    else
                    {
                        SelectionStart = p;
                        SelectionEnd = _selectionAnchor;
                    }

                    //Invalidate regions where selection have changed
                    if (p.Line == _selectionLast.Line)
                        InvalidateLine(p.Line);
                    else if (p.Line > _selectionLast.Line)
                    {
                        for (int i = _selectionLast.Line; i <= p.Line; i++)
                            InvalidateLine(i);
                    }
                    else
                    {
                        for (int i = p.Line; i <= _selectionLast.Line; i++)
                            InvalidateLine(i);
                    }

                    _selectionLast = p;
                    CaretPos = p;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_bSelectionInProgress)
                {
                    _bSelectionInProgress = false;

                    TextPos p = CordsToTextPos(e.Location);
                    CaretPos = p;

                    if (p > _selectionAnchor)
                    {
                        SelectionStart = _selectionAnchor;
                        SelectionEnd = p;
                    }
                    else
                    {
                        SelectionStart = p;
                        SelectionEnd = _selectionAnchor;
                    }

                    //Invalidate regions where selection have changed
                    if (p.Line == _selectionLast.Line)
                        InvalidateLine(p.Line);
                    else if (p.Line > _selectionLast.Line)
                    {
                        for (int i = _selectionLast.Line; i <= p.Line; i++)
                            InvalidateLine(i);
                    }
                    else
                    {
                        for (int i = p.Line; i <= _selectionLast.Line; i++)
                            InvalidateLine(i);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_charWidth == null)
            {
                InitCharWidth(e.Graphics);
                MeasureLineWidthAll();
                PositionCaret();
            }

            Rectangle clipRec = AdjustClipRectagleToLineBoundaries(e.ClipRectangle);

            EraseBackground(e.Graphics, clipRec);

            int iStart = GetStartDrawLine(clipRec.Y);
            int iEnd = GetEndDrawLine(clipRec.Y + clipRec.Height);
            
            for (int i = iStart; i <= iEnd; i++)
            {
                DrawLine(e.Graphics, i, clipRec.X, clipRec.Width);
                if (_bShowLineNumbers)
                    DrawLineNumber(e.Graphics, i, clipRec.X, clipRec.Width);
                if (_bShowIconField)
                    DrawIcons(e.Graphics, i);
            }

            base.OnPaint(e);
        }

        private void EraseBackground(Graphics g, Rectangle rec)
        {
            int xOffset = AutoScrollPosition.X;
            if (ShowLineNumbers)
            {
                Rectangle lineRec = rec;
                lineRec.X = xOffset;
                lineRec.Width = rec.X + _lineNumbersWidth;
                lineRec = Rectangle.Intersect(lineRec, rec);
                if (lineRec.Width > 0)
                {
                    g.FillRectangle(_sbLineBackColor, lineRec);
                }

                xOffset += _lineNumbersWidth;
            }

            if (_bShowIconField)
            {
                Rectangle iconRec = rec;
                iconRec.X = xOffset;
                iconRec.Width = iconRec.X + _iconFieldWidth;
                iconRec = Rectangle.Intersect(iconRec, rec);
                if (iconRec.Width > 0)
                {
                    g.FillRectangle(_sbIconBackColor, iconRec);
                }
                xOffset += _iconFieldWidth;
            }

            Rectangle textRec = new Rectangle(xOffset, 0, Width - xOffset, Height);
            textRec = Rectangle.Intersect(textRec, rec);
            if (textRec.Width > 0)
            {
                g.FillRectangle(_sbBackColor, textRec);
            }
        }

        private bool RangesOverlap(int x1, int width1, int x2, int width2)
        {
            return !((x1 + width1) < x2 || (x2 + width2) < x1);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void DrawLine(Graphics g, int line, int x, int width)
        {
            int yOffset = (line * _lineHeight) + AutoScrollPosition.Y;
            float xOffset = _lineIndent + AutoScrollPosition.X;
            if (_bShowLineNumbers)
                xOffset += _lineNumbersWidth;
            if (_bShowIconField)
                xOffset += _iconFieldWidth;

            StringDraw sd = new StringDraw(g);

            TextLine tl = _lineBuffer.GetLine(line);
            int lineIndex = 0;
            foreach (TextLineSegment tls in tl)
            {
                string s = tls.Text;
                SolidBrush fg = new SolidBrush(tls.FgColor);
                SolidBrush bg = new SolidBrush(tls.BgColor);

                for (int i = 0; i < s.Length; i++)
                {
                    float cWidth = GetCharWidth((int)s[i]);

                    if (RangesOverlap((int)xOffset, (int)cWidth, x, width))
                    {
                        if (IsTextPosInsideSelection(line, lineIndex))
                        {
                            g.FillRectangle(_sbBackColorSelection, new RectangleF(xOffset, yOffset, GetCharWidth(s[i]), _lineHeight));
                            sd.DrawString(s[i], Font, _sbBackColor, xOffset, yOffset);
                        }
                        else
                        {
                            if (tls.BgColorSet)
                            {
                                g.FillRectangle(bg, new RectangleF(xOffset, yOffset, GetCharWidth(s[i]), _lineHeight));
                            }

                            sd.DrawString(s[i], Font, fg, xOffset, yOffset);
                        }
                    }
                    xOffset += cWidth + _charSpacing;
                    if (xOffset > (x + width))
                        return;

                    lineIndex++;
                }
            }
        }

        private void DrawLineNumber(Graphics g, int line, int x, int width)
        {
            int yOffset = (line * _lineHeight) + AutoScrollPosition.Y;
            float xOffset = _lineNumbersWidth + AutoScrollPosition.X;

            string lineNum = line.ToString();
            StringDraw sd = new StringDraw(g);
            for (int i = lineNum.Length - 1; i >= 0; i--)
            {
                float cWidth = GetCharWidth((int)lineNum[i]);
                xOffset -= cWidth;
                if (RangesOverlap((int)xOffset, (int)cWidth, x, width))
                {
                    sd.DrawString(lineNum[i], Font, _sbLineForeColor, xOffset, yOffset);
                }
            }
        }

        private void DrawIcons(Graphics g, int line)
        {
        }

        private int GetStartDrawLine(int y)
        {
            int realY = y - AutoScrollPosition.Y;
            int line = realY / _lineHeight;

            if (line >= _lineBuffer.Count)
                line = _lineBuffer.Count - 1;

            return line;
        }

        private int GetEndDrawLine(int y)
        {
            int realY = y - AutoScrollPosition.Y;
            int line = realY / _lineHeight;

            if (line >= _lineBuffer.Count)
                line = _lineBuffer.Count - 1;

            return line;
        }

        private Rectangle AdjustClipRectagleToLineBoundaries(Rectangle rec)
        {
            //TODO: calculate
            return rec;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private void PositionCaret()
        {
            int y = (_lineHeight * CaretPos.Line) + AutoScrollPosition.Y;
            int x = (int)GetLineXPos(CaretPos.Line, CaretPos.Index);
            _textCaret.Position = new Point(x, y);
        }

        protected int LineHeight
        {
            get { return _lineHeight; }
        }

        public Point GetCaretLocation()
        {
            return _textCaret.Position;
        }

        private float GetLineXPos(int line, int index)
        {
            if (_charWidth == null)
                return 0;

            string lineText = _lineBuffer.GetLine(line).GetText(0, index);
            float xOffset = _lineIndent + AutoScrollPosition.X;
            for (int i = 0; i < lineText.Length; i++)
            {
                xOffset += GetCharWidth((int)lineText[i]) + _charSpacing;
            }

            if (_bShowLineNumbers)
                xOffset += _lineNumbersWidth;
            if (_bShowIconField)
                xOffset += _iconFieldWidth;

            return xOffset;
        }

        public float MeasureLineWidth(int line, bool bUpdateMaxWidth)
        {
            if (_charWidth == null)
                return 0;

            float width = _lineIndent;
            TextLine tl = _lineBuffer.GetLine(line);
            string str = tl.GetText();
            for (int i = 0; i < str.Length; i++)
            {
                width += GetCharWidth((byte)str[i]) + _charSpacing;
            }
            tl.LineWidth = width;

            if (bUpdateMaxWidth)
            {
                if (_bShowLineNumbers)
                {
                    UpdateLineNumWidth();
                    width += _lineNumbersWidth;
                }
                if (_bShowIconField)
                    width += _iconFieldWidth;

                if (width > _maxLineWidth)
                {
                    _maxLineWidth = (int)width;
                    SetAutoScrollMinSize();
                }
            }
            return width;
        }

        public void MeasureLineWidthAll()
        {
            float max = 0;
            for (int i = 0; i < _lineBuffer.Count; i++)
            {
                float current = MeasureLineWidth(i, false);
                if (current > max)
                    max = current;
            }

            if (_bShowLineNumbers)
            {
                UpdateLineNumWidth();
                max += _lineNumbersWidth;
            }
            if (_bShowIconField)
                max += _iconFieldWidth;

            if (max != _maxLineWidth)
            {
                _maxLineWidth = (int)max;
                SetAutoScrollMinSize();
            }
        }

        private float GetCharWidth(int index)
        {
            if (index > 254 || _charWidth == null)
            {
                return 18;
            }
            return _charWidth[index];
        }

        private void InitCharWidth(Graphics g)
        {
            StringDraw sd = new StringDraw(g);
            _charWidth = new float[256];
            for (int i = 0; i < 256; i++)
            {
                _charWidth[i] = sd.MeasureString(((char)i).ToString(), Font).Width;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }
            return false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_acceptsKeyInput)
                return;

            if (e.KeyCode == Keys.Back)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnBackKey();
            }
            else if (e.KeyCode == Keys.Left)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnLeftKey();
            }
            else if (e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnRightKey();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnUpKey();
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnDownKey();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnEnterKey();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnTabKey();
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnCopy();
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnPaste();
            }
            else if (e.KeyCode == Keys.X && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnCut();
            }
            else if (e.KeyCode == Keys.Z && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnUndo();
            }
            else if (e.KeyCode == Keys.Y && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                OnRedo();
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!_acceptsKeyInput)
                return;

            if (!Char.IsControl(e.KeyChar))
            {
                e.Handled = true;

                OnKeyPressed(e.KeyChar);
                OnAfterKeyPressed(CaretPos.Line, CaretPos.Index, e.KeyChar);
                if (TextInserted != null)
                    TextInserted(this, new TextPos(CaretPos.Line, CaretPos.Index - 1), new TextPos(CaretPos.Line, CaretPos.Index), e.KeyChar.ToString());
            }

            InvalidateSelection();
            SelectionEnd = SelectionStart;
        }

        protected virtual void OnKeyPressed(char c)
        {
            if (SelectionStart < SelectionEnd)
            {
                ReplaceText(SelectionStart, SelectionEnd, c.ToString(), true);
                CaretPos = new TextPos(SelectionStart.Line, SelectionStart.Index + 1);
            }
            else
            {
                InsertTextAtPos(CaretPos, c.ToString(), true, true);
            }
        }

        protected virtual void OnAfterKeyPressed(int line, int index, char c)
        {
        }

        protected virtual void OnCopy()
        {
            string selText = GetSelectedText();
            if (selText.Length > 0)
            {
                int retryCount = 10;
                if (retryCount > 0)
                {
                    while (retryCount > 0)
                    {
                        try
                        {
                            Clipboard.ContainsText();
                            Clipboard.SetText(selText, TextDataFormat.UnicodeText);
                            break;
                        }
                        catch (Exception)
                        {
                            retryCount--;
                            if (retryCount == 0)
                            {
                                MessageBox.Show("Failed to copy text to clipboard");
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(10);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnCut()
        {
            if (IsTextSelected())
            {
                OnCopy();
                _undoHistory.AddUndoRecord(new UndoRecord(SelectionStart, SelectionEnd, "", false));
                RemoveText(SelectionStart, SelectionEnd, true);
                SelectionEnd = SelectionStart;
                InvalidateSelection();
            }
        }

        protected virtual void OnUndo()
        {
            if (IsTextSelected())
            {
                SelectionEnd = SelectionStart;
            }

            var undoRecord = _undoHistory.GetNextUndo();
            if (undoRecord == null)
            {
                return;
            }

            if (!IsValidTextPos(undoRecord.Start))
            {
                return;
            }

            if (undoRecord.IsInsert)
            {
                if (!IsValidTextPos(undoRecord.End))
                {
                    return;
                }
                RemoveText(undoRecord.Start, undoRecord.End, true, false);
            }
            else
            {
                InsertTextAtPos(undoRecord.Start, undoRecord.Text, false);
            }
        }

        protected virtual void OnRedo()
        {
            if (IsTextSelected())
            {
                SelectionEnd = SelectionStart;
            }

            var redoRecord = _undoHistory.GetNextRedo();
            if (redoRecord == null)
            {
                return;
            }

            if (!IsValidTextPos(redoRecord.Start))
            {
                return;
            }

            if (redoRecord.IsInsert)
            {
                InsertTextAtPos(redoRecord.Start, redoRecord.Text, false);
            }
            else
            {
                if (!IsValidTextPos(redoRecord.End))
                {
                    return;
                }
                RemoveText(redoRecord.Start, redoRecord.End, false);
            }
        }

        private bool IsValidTextPos(TextPos pos)
        {
            if (pos.Line >= _lineBuffer.Count)
            {
                return false;
            }
                
            var line = _lineBuffer.GetLine(pos.Line);
            if (line.Length < pos.Index)
            {
                return false;
            }

            return true;
        }

        protected virtual void OnPaste()
        {
            string insText = Clipboard.GetText();
            OnPaste(insText);
        }

        protected virtual void OnPaste(string insText)
        {
            if (IsTextSelected())
            {
                TextPos end = SelectionEnd;
                SelectionEnd = SelectionStart;
                ReplaceText(SelectionStart, end, insText, true);
            }
            else
            {
                InsertTextAtPos(_caretPos, insText, true);
            }
        }

        public string GetSelectedText()
        {
            if (IsTextSelected())
            {
                if (SelectionStart.Line == SelectionEnd.Line)
                {
                    return _lineBuffer.GetLine(SelectionStart.Line).GetText(SelectionStart.Index, SelectionEnd.Index - SelectionStart.Index);
                }
                else
                {
                    StringBuilder sbRet = new StringBuilder();
                    for (int i = SelectionStart.Line; i <= SelectionEnd.Line; i++)
                    {
                        TextLine tl = _lineBuffer.GetLine(i);
                        if (i == SelectionStart.Line)
                        {
                            sbRet.Append(tl.GetText(SelectionStart.Index, tl.Length - SelectionStart.Index));
                        }
                        else if (i == SelectionEnd.Line)
                        {
                            sbRet.Append(Environment.NewLine + tl.GetText(0, SelectionEnd.Index));
                        }
                        else
                        {
                            sbRet.Append(Environment.NewLine + tl.GetText());
                        }
                    }
                    return sbRet.ToString();
                }
            }
            return "";
        }

        public int RemoveText(TextPos startPos, TextPos endPos, bool invalidate, bool enableUndoRedo = true)
        {
            if (endPos <= startPos)
                return 0;

            if (enableUndoRedo)
            {
                _undoHistory.AddUndoRecord(new UndoRecord(startPos, endPos, "", false));
            }

            for (int i = endPos.Line; i >= startPos.Line; i--)
            {
                if (i == startPos.Line)
                {
                    TextLine tl = _lineBuffer.GetLine(startPos.Line);
                    int length = tl.Length - startPos.Index;
                    if (i == endPos.Line)
                        length = endPos.Index - startPos.Index;
                    tl.RemoveText(startPos.Index, length);
                }
                else if (i == endPos.Line)
                {
                    TextLine tl = _lineBuffer.GetLine(endPos.Line);
                    if (endPos.Index >= tl.Length)
                    {
                        _lineBuffer.DeleteLine(endPos.Line);
                    }
                    else
                    {
                        tl.RemoveText(0, endPos.Index);
                    }
                }
                else
                {
                    _lineBuffer.DeleteLine(i);
                }
            }

            int linesInvolved = endPos.Line - startPos.Line;
            if (invalidate)
            {
                if (linesInvolved > 0)
                    InvalidateFromLine(startPos.Line);
                else
                    InvalidateLine(startPos.Line);
                SetAutoScrollMinSize();
            }

            if (TextInserted != null)
                TextInserted(this, startPos, startPos, "");

            CaretPos = startPos;
            return linesInvolved + 1;
        }

        public int ReplaceText(TextPos startPos, TextPos endPos, string text, bool invalidate, bool enableUndoRedo = true)
        {
            int linesRemoved = RemoveText(startPos, endPos, false, enableUndoRedo);
            
            var newEndPos = InsertTextAtPos(startPos, text, false, enableUndoRedo);
            int linesInserted = GetNumLines(startPos, newEndPos);
            if (invalidate)
            {
                if (linesRemoved > 1 || linesInserted > 1)
                    InvalidateFromLine(startPos.Line);
                else
                    InvalidateLine(startPos.Line);
                SetAutoScrollMinSize();
            }

            if (linesInserted > linesRemoved)
                return linesInserted;
            return linesRemoved;
        }

        private int GetNumLines(TextPos startPos, TextPos endPos)
        {
            return endPos.Line - startPos.Line;
        }

        public TextPos InsertTextAtPos(TextPos pos, string text, bool invalidate, bool enableUndoRedo = true)
        {
            if (text == null || text.Length < 1)
                return pos;

            TextPos posStart = pos;
            
            string[] insText = text.Replace("\r", "").Split(new char[] { '\n' });
            int invalidateFrom = pos.Line;
            for (int i = 0; i < insText.Length; i++)
            {
                if (i == 0)
                {
                    TextLine tl = _lineBuffer.GetLine(pos.Line);
                    tl.InsertText(pos.Index, insText[i]);
                    pos.Index += insText[i].Length;
                    MeasureLineWidth(pos.Line, true);
                }
                else
                {
                    pos.Line++;
                    TextLine tl = _lineBuffer.InsertLine(pos.Line);
                    tl.AppendText(insText[i]);
                    pos.Index = tl.Length;
                    MeasureLineWidth(pos.Line, true);
                }
            }

            if (invalidate)
            {
                if (insText.Length > 1)
                    InvalidateFromLine(invalidateFrom);
                else
                    InvalidateLine(invalidateFrom);
                SetAutoScrollMinSize();
            }

            if (TextInserted != null)
                TextInserted(this, posStart, pos, text);

            if (enableUndoRedo)
            {
                _undoHistory.AddUndoRecord(new UndoRecord(posStart, pos, text, true));
            }

            CaretPos = pos;
            return pos;
        }

        public bool IsTextSelected()
        {
            if (SelectionStart != SelectionEnd && SelectionStart < SelectionEnd)
            {
                return true;
            }
            return false;
        }

        protected virtual void OnUpKey()
        {
            if (CaretPos.Line > 0)
            {
                int upLineLength = _lineBuffer.GetLine(CaretPos.Line - 1).Length;
                CaretPos = new TextPos(CaretPos.Line - 1, CaretPos.Index > upLineLength ? upLineLength : CaretPos.Index);
            }
            else
                return;

            if (!IsCaretInsideScreen())
            {
                ScrollLines(-1);
            }
        }

        protected virtual void OnDownKey()
        {
            if (CaretPos.Line < (_lineBuffer.Count - 1))
            {
                int downLineLength = _lineBuffer.GetLine(CaretPos.Line + 1).Length;
                CaretPos = new TextPos(CaretPos.Line + 1, CaretPos.Index > downLineLength ? downLineLength : CaretPos.Index);
            }
            else
                return;

            if (!IsCaretInsideScreen())
            {
                ScrollLines(1);
            }
        }

        protected virtual void Beep()
        {
            //System.Console.Beep();
        }

        protected virtual void OnLeftKey()
        {
            if (CaretPos.Index > 0)
            {
                CaretPos = new TextPos(CaretPos.Line, CaretPos.Index - 1);
            }
            else if (CaretPos.Line > 0)
            {
                CaretPos = new TextPos(CaretPos.Line - 1, _lineBuffer.GetLine(CaretPos.Line - 1).Length);
            }
            else
            {
                Beep();
                return;
            }
        }

        protected virtual void OnRightKey()
        {
            if (CaretPos.Index < _lineBuffer.GetLine(CaretPos.Line).Length)
            {
                CaretPos = new TextPos(CaretPos.Line, CaretPos.Index + 1);
            }
            else if (CaretPos.Line < (_lineBuffer.Count - 1))
            {
                CaretPos = new TextPos(CaretPos.Line + 1, 0);
            }
            else
            {
                Beep();
                return;
            }
        }

        protected virtual void OnTabKey()
        {
            TextLine tlCurrent = _lineBuffer.GetLine(CaretPos.Line);
            if (_treatTabAsSpaces)
            {
                for (int i = 0; i < _numSpacesForTab; i++)
                {
                    tlCurrent.InsertText(CaretPos.Index, " ");
                    CaretPos = new TextPos(CaretPos.Line, CaretPos.Index + 1);
                }
            }
            else
            {
                tlCurrent.InsertText(CaretPos.Index, "\t");
                CaretPos = new TextPos(CaretPos.Line, CaretPos.Index + 1);
            }
            MeasureLineWidth(CaretPos.Line, true);
            InvalidateLine(CaretPos.Line);
        }

        protected virtual void OnBackKey()
        {
            if (IsTextSelected())
            {
                ReplaceText(SelectionStart, SelectionEnd, "", true, true);
                CaretPos = SelectionStart;
                SelectionEnd = SelectionStart;
            }
            else
            {
                if (CaretPos.Index > 0)
                {
                    RemoveText(new TextPos(CaretPos.Line, CaretPos.Index - 1), CaretPos, true, true);
                }
                else if (CaretPos.Line > 0)
                {
                    RemoveText(new TextPos(CaretPos.Line - 1, GetLineLength(CaretPos.Line - 1)), CaretPos, true, true);
                }
                else
                {
                    Beep();
                    return;
                }
            }
        }

        protected virtual void OnEnterKey()
        {
            TextLine tlCurrent = _lineBuffer.GetLine(CaretPos.Line);
            if (CaretPos.Index < tlCurrent.Length)
            {
                TextLine tlNew = new TextLine(tlCurrent.GetText(CaretPos.Index, tlCurrent.Length - CaretPos.Index), ForeColor);
                List<TextLineFormating> formating = tlCurrent.GetFormating();
                foreach (TextLineFormating format in formating)
                {
                    if ((format.Index + format.Length) >= CaretPos.Index)
                    {
                        int diff = format.Index - CaretPos.Index;
                        if (diff < 0)
                        {
                            format.Length += diff;
                            format.Index -= diff;
                        }
                        if (format.BgColorSet)
                            tlNew.AppendFormating(diff, format.Length, format.FgColor, format.BgColor);
                        else
                            tlNew.AppendFormating(diff, format.Length, format.FgColor);
                    }
                }
                tlCurrent.RemoveText(CaretPos.Index, tlCurrent.Length - CaretPos.Index);
                _lineBuffer.InsertLine(CaretPos.Line + 1).AppendText(tlNew);
                InvalidateFromLine(CaretPos.Line);
                CaretPos = new TextPos(CaretPos.Line + 1, 0);
            }
            else
            {
                _lineBuffer.InsertLine(CaretPos.Line + 1);
                CaretPos = new TextPos(CaretPos.Line + 1, 0);
                InvalidateFromLine(CaretPos.Line);
            }

            UpdateLineNumWidth();
            SetAutoScrollMinSize();
            if (!IsCaretInsideScreen())
                ScrollToCaret();
        }
    }
}
