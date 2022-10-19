using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

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
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this._lineHeight = (int)base.Font.GetHeight() + this._lineSpacing;
            this._lineBuffer = new LineBuffer(this.ForeColor);
            this._lineBuffer.InsertLine(0);
            this._textCaret = new TextCaret(this);
            this._textCaret.Size = new Size(1, this._lineHeight);

            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.ForeColorLineNum = Color.FromArgb(64, 64, 64);
            this.BackColorLineNum = Color.FromArgb(224, 224, 224);
            this.BackColorIconPane = Color.Yellow;
            this.BackColorSelection = Color.FromArgb(122, 150, 223); 

            this.SetAutoScrollMinSize();

            this.Cursor = Cursors.IBeam;

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            MenuItem miCut = new MenuItem("Cut");
            miCut.Click += new EventHandler(miCut_Click);
            this.ContextMenu.MenuItems.Add(miCut);

            MenuItem miCopy = new MenuItem("Copy");
            miCopy.Click += new EventHandler(miCopy_Click);
            this.ContextMenu.MenuItems.Add(miCopy);

            MenuItem miPaste = new MenuItem("Paste");
            miPaste.Click += new EventHandler(miPaste_Click);
            this.ContextMenu.MenuItems.Add(miPaste);
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            if (this.SelectionStart < this.SelectionEnd)
            {
                this.ContextMenu.MenuItems[0].Enabled = true;
                this.ContextMenu.MenuItems[1].Enabled = true;
            }
            else
            {
                this.ContextMenu.MenuItems[0].Enabled = false;
                this.ContextMenu.MenuItems[1].Enabled = false;
            }

            if (Clipboard.ContainsText())
            {
                this.ContextMenu.MenuItems[2].Enabled = true;
            }
            else
            {
                this.ContextMenu.MenuItems[2].Enabled = false;
            }
        }

        void miPaste_Click(object sender, EventArgs e)
        {
            this.OnPaste();
        }

        void miCopy_Click(object sender, EventArgs e)
        {
            this.OnCopy();
        }

        void miCut_Click(object sender, EventArgs e)
        {
            this.OnCut();
        }

        public void Cut()
        {
            this.OnCut();
        }

        public void Copy()
        {
            this.OnCopy();
        }

        public void Paste()
        {
            this.OnPaste();
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
                this._lineHeight = (int)base.Font.GetHeight() + this._lineSpacing;
                this._textCaret.Size = new Size(1, this._lineHeight);
                this._charWidth = null;
                this.SetAutoScrollMinSize();
                this.Invalidate();
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
                this._sbBackColor = new SolidBrush(this.BackColor);
                this.Invalidate();
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
                this._sbForeColor = new SolidBrush(this.ForeColor);
                this._lineBuffer.ForeColor = value;
                this.Invalidate();
            }
        }

        public Color ForeColorLineNum
        {
            get { return this._sbLineForeColor.Color; }
            set 
            { 
                this._sbLineForeColor = new SolidBrush(value);
                if (this._bShowLineNumbers)
                    this.Invalidate();
            }
        }

        public Color BackColorLineNum
        {
            get { return this._sbLineBackColor.Color; }
            set
            {
                this._sbLineBackColor = new SolidBrush(value);
                if (this._bShowLineNumbers)
                    this.Invalidate();
            }
        }

        public Color BackColorSelection
        {
            get { return this._sbBackColorSelection.Color; }
            set 
            {
                this._sbBackColorSelection = new SolidBrush(value);
                if (this.IsTextSelected())
                    this.Invalidate();
            }
        }

        public bool ShowLineNumbers
        {
            get { return this._bShowLineNumbers; }
            set
            {
                if (this._bShowLineNumbers != value)
                {
                    this._bShowLineNumbers = value;
                    this.PositionCaret();
                    this.Invalidate();
                }
            }
        }

        public Color BackColorIconPane
        {
            get { return this._sbIconBackColor.Color; }
            set 
            { 
                this._sbIconBackColor = new SolidBrush(value);
                if (this._bShowIconField)
                    this.Invalidate();
            }
        }

        public bool ShowIconPane
        {
            get { return this._bShowIconField; }
            set
            {
                if (this._bShowIconField != value)
                {
                    this._bShowIconField = value;
                    this.PositionCaret();
                    this.Invalidate();
                }
            }
        }

        public bool AcceptsKeyInput
        {
            get { return this._acceptsKeyInput; }
            set { this._acceptsKeyInput = value; }
        }

        public override string Text
        {
            get
            {
                StringBuilder sbRet = new StringBuilder();
                for (int i = 0; i < this._lineBuffer.Count; i++)
                {
                    sbRet.Append(this._lineBuffer.GetLine(i).GetText());
                }
                return sbRet.ToString();
            }
            set
            {
                this._lineBuffer.Clear();
                string[] lines = value.Replace("\r", "").Split(new char[] {'\n'});
                for (int i = 0; i < lines.Length; i++)
                {
                    this._lineBuffer.InsertLine(i).InsertText(0, lines[i]);
                }

                if (lines.Length < 1)
                    this._lineBuffer.InsertLine(0);

                this.CaretPos = new TextPos(0, 0);
                this.AutoScrollPosition = new Point(0, 0);
                this.MeasureLineWidthAll();
                this.InvalidateFromLine(0);
            }
        }

        public string[] Lines
        {
            get
            {
                string[] ret = new string[this._lineBuffer.Count];
                for (int i = 0; i < this._lineBuffer.Count; i++)
                    ret[i] = this._lineBuffer.GetLine(i).GetText();
                return ret;
            }
        }

            public void Clear()
        {
            this._lineBuffer.Clear();
            this._lineBuffer.InsertLine(0);
            this.CaretPos = new TextPos(0, 0);
            this.AutoScrollPosition = new Point(0, 0);
            this.MeasureLineWidthAll();
            this.InvalidateFromLine(0);
        }

        public void BeginUpdate()
        {
            this._inUpdate = true;
        }

        public void EndUpdate()
        {
            this._inUpdate = false;
            this.Invalidate();
        }

        public void SetText(int line, string text)
        {
            TextLine tl = this._lineBuffer.GetLine(line);
            tl.RemoveText();
            tl.AppendText(text);
            this.InvalidateLine(line);
        }

        public void InsertLine(int line, string text)
        {
            this._lineBuffer.InsertLine(line).InsertText(0, text);
            this.InvalidateFromLine(line);
            this.SetAutoScrollMinSize();
        }

        public void RemoveLine(int line)
        {
            this._lineBuffer.DeleteLine(line);
            this.InvalidateFromLine(line);
            this.SetAutoScrollMinSize();
        }

        public void AppendText(string text)
        {
            TextPos pos = new TextPos(this._lineBuffer.Count - 1, this._lineBuffer.GetLine(this._lineBuffer.Count - 1).Length);
            this.InsertTextAtPos(pos, text, true);
        }

        public void InsertTextAtCaret(string text)
        {
            this.InsertTextAtPos(this.CaretPos, text, true);
        }

        public string GetText(int line)
        {
            return this._lineBuffer.GetLine(line).GetText();
        }

        public string GetText(int line, int index, int length)
        {
            return this._lineBuffer.GetLine(line).GetText(index, length);
        }

        public string GetText(TextPos start, TextPos end)
        {
            if (start >= end)
                return "";

            if (start.Line == end.Line)
            {
                return this._lineBuffer.GetLine(start.Line).GetText(start.Index, end.Index - start.Index);
            }

            StringBuilder sbRet = new StringBuilder();
            for (int i = start.Line; i <= end.Line; i++)
            {
                TextLine tl = this._lineBuffer.GetLine(i);
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
            get { return this._lineBuffer.Count; }
        }

        public int GetLineLength(int line)
        {
            return this._lineBuffer.GetLine(line).Length;
        }

        public TextPos CaretPos
        {
            get { return this._caretPos; }
            set
            {
                if (this.OnBeforeCaretMove(this._caretPos, value))
                {
                    this._caretPos = value;
                    if (_caretPos.Line < 0 || _caretPos.Line >= _lineBuffer.Count)
                        throw new ArgumentOutOfRangeException("Invalid line number");
                    if (_caretPos.Index < 0 || _caretPos.Index > _lineBuffer.GetLine(_caretPos.Line).Length)
                        throw new ArgumentOutOfRangeException("Invadlid index on line");
                    this.PositionCaret();
                }
            }
        }

        public void ScrollToCaret()
        {
            int line = this.CaretPos.Line;
            int y = line * this._lineHeight;
            this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X, y);
        }

        protected virtual bool OnBeforeCaretMove(TextPos oldPos, TextPos newPos)
        {
            return true;
        }

        public TextPos GetTextEnd()
        {
            return new TextPos(this._lineBuffer.Count - 1, this._lineBuffer.GetLine(this._lineBuffer.Count - 1).Length);
        }

        public TextPos SelectionStart
        {
            get { return this._selectionStart; }
            set 
            { 
                this._selectionStart = value; 
            }
        }

        public TextPos SelectionEnd
        {
            get { return this._selectionEnd; }
            set 
            { 
                this._selectionEnd = value; 
            }
        }

        public void AddFormating(int line, int index, int length, Color textColor)
        {
            this._lineBuffer.GetLine(line).AppendFormating(index, length, textColor);
        }

        public void AddFormating(int line, int index, int length, Color textColor, Color bgColor)
        {
            this._lineBuffer.GetLine(line).AppendFormating(index, length, textColor, bgColor);
        }

        public void RemoveFormating(int line)
        {
            this._lineBuffer.GetLine(line).RemoveFormating();
        }

        public void RemoveFormating(int line, int index, int length)
        {
            this._lineBuffer.GetLine(line).RemoveFormating(index, length);
        }

        private void UpdateLineNumWidth()
        {
            if (!this._bShowLineNumbers || this._charWidth == null)
                return;

            int max = this._lineBuffer.Count - 1;
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
            if (this._lineNumbersWidth != iWidth)
            {
                this._lineNumbersWidth = iWidth;
                this.Invalidate();
            }
        }

        private void SetAutoScrollMinSize()
        {
            this.AutoScrollMinSize = new Size(this._maxLineWidth + 10, (this._lineBuffer.Count * this._lineHeight) + 10);
        }

        public void ScrollLines(int lines)
        {
            this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y + (lines * this._lineHeight));
        }

        private bool IsCaretInsideScreen()
        {
            if (this._textCaret.Position.Y < 0 || this._textCaret.Position.Y > (this.ClientRectangle.Bottom - (this._lineHeight + 1)))
                return false;
            if (this._textCaret.Position.X < 1 || this._textCaret.Position.X > (this.ClientRectangle.Right - 1))
                return false;
            return true;
        }

        private bool IsCordsInsideTextArea(Point p)
        {
            int x = this.AutoScrollPosition.X;
            if (this._bShowLineNumbers)
                x += this._lineNumbersWidth;
            if (this._bShowIconField)
                x += this._iconFieldWidth;

            if (p.X > x)
                return true;

            return false;
        }

        private bool IsCordsInsideLineNumArea(Point p)
        {
            if (!this._bShowLineNumbers)
                return false;
            if (p.X < (this._lineNumbersWidth + this.AutoScrollPosition.X))
                return true;
            return false;
        }

        private bool IsCordsInsideIconArea(Point p)
        {
            if (!this._bShowIconField)
                return false;
            int xMin = this.AutoScrollPosition.X;
            if (this._bShowLineNumbers)
                xMin += this._lineNumbersWidth;
            int xMax = xMin + this._iconFieldWidth;
            if (p.X >= xMin && p.X < xMax)
                return true;
            return false;
        }

        private int CordsToLine(int y)
        {
            int line = (y - this.AutoScrollPosition.Y) / this._lineHeight;
            if (line >= this._lineBuffer.Count)
                line = this._lineBuffer.Count - 1;
            if (line < 0)
                line = 0;
            return line;
        }

        private TextPos CordsToTextPos(Point p)
        {
            int line = this.CordsToLine(p.Y);
            int index = 0;

            if (line >= this._lineBuffer.Count)
            {
                return new TextPos(0, index);
            }
            else if (line < 0)
            {
                return new TextPos(0, index);
            }

            string s = this._lineBuffer.GetLine(line).GetText();
            float xOffset = this._lineIndent + this.AutoScrollPosition.X;
            if (this._bShowLineNumbers)
                xOffset += this._lineNumbersWidth;
            if (this._bShowIconField)
                xOffset += this._iconFieldWidth;

            for (int i = 0; i < s.Length; i++)
            {
                float width = GetCharWidth((int)s[i]);
                if (p.X < (xOffset + (width / 2)))
                {
                    return new TextPos(line, i);
                }
                xOffset += width + this._charSpacing;
            }

            return new TextPos(line, s.Length);
        }

        private void InvalidateSelection()
        {
            if (this._inUpdate)
                return;

            for (int i = this.SelectionStart.Line; i <= this.SelectionEnd.Line; i++ )
            {
                this.InvalidateLine(i);
            }
        }

        private void InvalidateFromLine(int line)
        {
            if (this._inUpdate)
                return;

            int beginY = (line * this._lineHeight) + this.AutoScrollPosition.Y;
            this.Invalidate(new Rectangle(0, beginY, this.ClientRectangle.Width, this.ClientRectangle.Height - beginY));
        }

        public void InvalidateLine(int line)
        {
            if (this._inUpdate)
                return;

            int y = (line * this._lineHeight) + this.AutoScrollPosition.Y;
            int x = this.AutoScrollPosition.X;
            if (this._bShowLineNumbers)
                x += this._lineNumbersWidth;
            if (this._bShowIconField)
                x += this._iconFieldWidth;

            this.Invalidate(new Rectangle(x, y, this._maxLineWidth - x, this._lineHeight));
        }

        private bool IsTextPosInsideSelection(int line, int index)
        {
            if (this.SelectionStart == this.SelectionEnd)
                return false;
            if (line < this.SelectionStart.Line || line > this.SelectionEnd.Line)
                return false;

            if (line > this.SelectionStart.Line && line < this.SelectionEnd.Line)
                return true;

            if (line == this.SelectionStart.Line && index < this.SelectionStart.Index)
                return false;
            if (line == this.SelectionEnd.Line && index >= this.SelectionEnd.Index)
                return false;

            return true;
        }

        private int FindLefWordBoundary(int line, int index)
        {
            string s = this._lineBuffer.GetLine(line).GetText();
            for (int i = index - 1; i >= 0; i--)
            {
                if (this.CharIsSeperatorChar(s[i]))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private int FindRightWordBoundary(int line, int index)
        {
            string s = this._lineBuffer.GetLine(line).GetText();
            for (int i = index; i < s.Length; i++)
            {
                if (this.CharIsSeperatorChar(s[i]))
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
                if (this.IsCordsInsideTextArea(e.Location))
                {
                    if (TrySelectionInsideString(CaretPos.Line, CaretPos.Index))
                    {
                        this._bSelectionInProgress = false;
                        return;
                    }

                    int left = this.FindLefWordBoundary(this.CaretPos.Line, this.CaretPos.Index);
                    int right = this.FindRightWordBoundary(this.CaretPos.Line, this.CaretPos.Index);
                    if (left < right)
                    {
                        this.SelectionStart = new TextPos(this.CaretPos.Line, left);
                        this.SelectionEnd = new TextPos(this.CaretPos.Line, right);
                        this.CaretPos = this.SelectionEnd;
                        this.InvalidateLine(this.CaretPos.Line);
                    }
                    else
                    {
                        this.SelectionStart = this._caretPos;
                        this.SelectionEnd = this._caretPos;
                    }
                    this._bSelectionInProgress = false;
                }
                else if (this.IsCordsInsideIconArea(e.Location))
                {
                    if (this.IconLineDoubleClicked != null)
                        this.IconLineDoubleClicked(this, this.CordsToLine(e.Y));
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
                        this.SelectionStart = new TextPos(this.CaretPos.Line, stringBegin + 1);
                        this.SelectionEnd = new TextPos(this.CaretPos.Line, i);
                        this.CaretPos = this.SelectionEnd;
                        this.InvalidateLine(this.CaretPos.Line);
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
                if (this.IsCordsInsideIconArea(e.Location))
                {
                    if (this.IconLineClicked != null)
                        this.IconLineClicked(this, this.CordsToLine(e.Y));
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.Focused)
                this.Focus();

            if (e.Button == MouseButtons.Left)
            {
                if (this.IsCordsInsideTextArea(e.Location))
                {
                    this._bSelectionInProgress = true;
                    this.InvalidateSelection();
                    this._selectionAnchor = this.CordsToTextPos(e.Location);
                    this._selectionLast = this._selectionAnchor;
                    this.SelectionStart = this._selectionAnchor;
                    this.SelectionEnd = this._selectionAnchor;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this._bSelectionInProgress)
                {
                    if (e.Y > this.ClientRectangle.Height)
                    {
                        int diff = e.Y - this.ClientRectangle.Height;
                        this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y + diff);
                    }
                    else if (e.Y < 0)
                    {
                        this.AutoScrollPosition = new Point(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y + e.Y);
                    }

                    TextPos p = this.CordsToTextPos(e.Location);

                    if (p == this._selectionLast)
                        return;

                    if (p > this._selectionAnchor)
                    {
                        this.SelectionStart = this._selectionAnchor;
                        this.SelectionEnd = p;
                    }
                    else
                    {
                        this.SelectionStart = p;
                        this.SelectionEnd = this._selectionAnchor;
                    }

                    //Invalidate regions where selection have changed
                    if (p.Line == this._selectionLast.Line)
                        this.InvalidateLine(p.Line);
                    else if (p.Line > this._selectionLast.Line)
                    {
                        for (int i = this._selectionLast.Line; i <= p.Line; i++)
                            this.InvalidateLine(i);
                    }
                    else
                    {
                        for (int i = p.Line; i <= this._selectionLast.Line; i++)
                            this.InvalidateLine(i);
                    }

                    this._selectionLast = p;
                    this.CaretPos = p;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this._bSelectionInProgress)
                {
                    this._bSelectionInProgress = false;

                    TextPos p = this.CordsToTextPos(e.Location);
                    this.CaretPos = p;

                    if (p > this._selectionAnchor)
                    {
                        this.SelectionStart = this._selectionAnchor;
                        this.SelectionEnd = p;
                    }
                    else
                    {
                        this.SelectionStart = p;
                        this.SelectionEnd = this._selectionAnchor;
                    }

                    //Invalidate regions where selection have changed
                    if (p.Line == this._selectionLast.Line)
                        this.InvalidateLine(p.Line);
                    else if (p.Line > this._selectionLast.Line)
                    {
                        for (int i = this._selectionLast.Line; i <= p.Line; i++)
                            this.InvalidateLine(i);
                    }
                    else
                    {
                        for (int i = p.Line; i <= this._selectionLast.Line; i++)
                            this.InvalidateLine(i);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._charWidth == null)
            {
                this.InitCharWidth(e.Graphics);
                this.MeasureLineWidthAll();
                this.PositionCaret();
            }

            Rectangle clipRec = this.AdjustClipRectagleToLineBoundaries(e.ClipRectangle);

            this.EraseBackground(e.Graphics, clipRec);

            int iStart = this.GetStartDrawLine(clipRec.Y);
            int iEnd = this.GetEndDrawLine(clipRec.Y + clipRec.Height);
            
            for (int i = iStart; i <= iEnd; i++)
            {
                this.DrawLine(e.Graphics, i, clipRec.X, clipRec.Width);
                if (this._bShowLineNumbers)
                    this.DrawLineNumber(e.Graphics, i, clipRec.X, clipRec.Width);
                if (this._bShowIconField)
                    this.DrawIcons(e.Graphics, i);
            }

            base.OnPaint(e);
        }

        private void EraseBackground(Graphics g, Rectangle rec)
        {
            int xOffset = this.AutoScrollPosition.X;
            if (this.ShowLineNumbers)
            {
                Rectangle lineRec = rec;
                lineRec.X = xOffset;
                lineRec.Width = rec.X + this._lineNumbersWidth;
                lineRec = Rectangle.Intersect(lineRec, rec);
                if (lineRec.Width > 0)
                {
                    g.FillRectangle(this._sbLineBackColor, lineRec);
                }

                xOffset += this._lineNumbersWidth;
            }

            if (this._bShowIconField)
            {
                Rectangle iconRec = rec;
                iconRec.X = xOffset;
                iconRec.Width = iconRec.X + this._iconFieldWidth;
                iconRec = Rectangle.Intersect(iconRec, rec);
                if (iconRec.Width > 0)
                {
                    g.FillRectangle(this._sbIconBackColor, iconRec);
                }
                xOffset += this._iconFieldWidth;
            }

            Rectangle textRec = new Rectangle(xOffset, 0, this.Width - xOffset, this.Height);
            textRec = Rectangle.Intersect(textRec, rec);
            if (textRec.Width > 0)
            {
                g.FillRectangle(this._sbBackColor, textRec);
            }
        }

        private bool RangesOverlap(int x1, int width1, int x2, int width2)
        {
            return !((x1 + width1) < x2 || (x2 + width2) < x1);
        }


        private void DrawLine(Graphics g, int line, int x, int width)
        {
            int yOffset = (line * this._lineHeight) + this.AutoScrollPosition.Y;
            float xOffset = this._lineIndent + this.AutoScrollPosition.X;
            if (this._bShowLineNumbers)
                xOffset += this._lineNumbersWidth;
            if (this._bShowIconField)
                xOffset += this._iconFieldWidth;

            StringDraw sd = new StringDraw(g);

            TextLine tl = this._lineBuffer.GetLine(line);
            int lineIndex = 0;
            foreach (TextLineSegment tls in tl)
            {
                string s = tls.Text;
                SolidBrush fg = new SolidBrush(tls.FgColor);
                SolidBrush bg = new SolidBrush(tls.BgColor);

                for (int i = 0; i < s.Length; i++)
                {
                    float cWidth = GetCharWidth((int)s[i]);

                    if (this.RangesOverlap((int)xOffset, (int)cWidth, x, width))
                    {
                        if (this.IsTextPosInsideSelection(line, lineIndex))
                        {
                            g.FillRectangle(this._sbBackColorSelection, new RectangleF(xOffset, yOffset, GetCharWidth((int)s[i]), this._lineHeight));
                            sd.DrawString(s[i], this.Font, this._sbBackColor, xOffset, yOffset);
                        }
                        else
                        {
                            if (tls.BgColorSet)
                            {
                                g.FillRectangle(bg, new RectangleF(xOffset, yOffset, GetCharWidth((int)s[i]), this._lineHeight));
                            }

                            sd.DrawString(s[i], this.Font, fg, xOffset, yOffset);
                        }
                    }
                    xOffset += cWidth + this._charSpacing;
                    if (xOffset > (x + width))
                        return;

                    lineIndex++;
                }
            }
        }

        private void DrawLineNumber(Graphics g, int line, int x, int width)
        {
            int yOffset = (line * this._lineHeight) + this.AutoScrollPosition.Y;
            float xOffset = this._lineNumbersWidth + this.AutoScrollPosition.X;

            string lineNum = line.ToString();
            StringDraw sd = new StringDraw(g);
            for (int i = lineNum.Length - 1; i >= 0; i--)
            {
                float cWidth = GetCharWidth((int)lineNum[i]);
                xOffset -= cWidth;
                if (this.RangesOverlap((int)xOffset, (int)cWidth, x, width))
                {
                    sd.DrawString(lineNum[i], this.Font, this._sbLineForeColor, xOffset, yOffset);
                }
            }
        }

        private void DrawIcons(Graphics g, int line)
        {
        }

        private int GetStartDrawLine(int y)
        {
            int realY = y - this.AutoScrollPosition.Y;
            int line = realY / this._lineHeight;

            if (line >= this._lineBuffer.Count)
                line = this._lineBuffer.Count - 1;

            return line;
        }

        private int GetEndDrawLine(int y)
        {
            int realY = y - this.AutoScrollPosition.Y;
            int line = realY / this._lineHeight;

            if (line >= this._lineBuffer.Count)
                line = this._lineBuffer.Count - 1;

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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        private void PositionCaret()
        {
            int y = (this._lineHeight * this.CaretPos.Line) + this.AutoScrollPosition.Y;
            int x = (int)this.GetLineXPos(this.CaretPos.Line, this.CaretPos.Index);
            this._textCaret.Position = new Point(x, y);
        }

        protected int LineHeight
        {
            get { return this._lineHeight; }
        }

        public Point GetCaretLocation()
        {
            return this._textCaret.Position;
        }

        private float GetLineXPos(int line, int index)
        {
            if (this._charWidth == null)
                return 0;

            string lineText = this._lineBuffer.GetLine(line).GetText(0, index);
            float xOffset = this._lineIndent + this.AutoScrollPosition.X;
            for (int i = 0; i < lineText.Length; i++)
            {
                xOffset += GetCharWidth((int)lineText[i]) + this._charSpacing;
            }

            if (this._bShowLineNumbers)
                xOffset += this._lineNumbersWidth;
            if (this._bShowIconField)
                xOffset += this._iconFieldWidth;

            return xOffset;
        }

        public float MeasureLineWidth(int line, bool bUpdateMaxWidth)
        {
            if (this._charWidth == null)
                return 0;

            float width = this._lineIndent;
            TextLine tl = this._lineBuffer.GetLine(line);
            string str = tl.GetText();
            for (int i = 0; i < str.Length; i++)
            {
                width += GetCharWidth((byte)str[i]) + this._charSpacing;
            }
            tl.LineWidth = width;

            if (bUpdateMaxWidth)
            {
                if (this._bShowLineNumbers)
                {
                    this.UpdateLineNumWidth();
                    width += this._lineNumbersWidth;
                }
                if (this._bShowIconField)
                    width += this._iconFieldWidth;

                if (width > this._maxLineWidth)
                {
                    this._maxLineWidth = (int)width;
                    this.SetAutoScrollMinSize();
                }
            }
            return width;
        }

        public void MeasureLineWidthAll()
        {
            float max = 0;
            for (int i = 0; i < this._lineBuffer.Count; i++)
            {
                float current = this.MeasureLineWidth(i, false);
                if (current > max)
                    max = current;
            }

            if (this._bShowLineNumbers)
            {
                this.UpdateLineNumWidth();
                max += this._lineNumbersWidth;
            }
            if (this._bShowIconField)
                max += this._iconFieldWidth;

            if (max != this._maxLineWidth)
            {
                this._maxLineWidth = (int)max;
                this.SetAutoScrollMinSize();
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
            this._charWidth = new float[256];
            for (int i = 0; i < 256; i++)
            {
                this._charWidth[i] = sd.MeasureString(((char)i).ToString(), this.Font).Width;
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
            if (!this._acceptsKeyInput)
                return;

            if (e.KeyCode == Keys.Back)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnBackKey();
            }
            else if (e.KeyCode == Keys.Left)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnLeftKey();
            }
            else if (e.KeyCode == Keys.Right)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnRightKey();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnUpKey();
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnDownKey();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnEnterKey();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnTabKey();
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnCopy();
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnPaste();
            }
            else if (e.KeyCode == Keys.X && e.Control)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                this.OnCut();
            }
            if (!e.Control && !e.Alt)
            {
                //this.InvalidateSelection();
                //this.SelectionEnd = this.SelectionStart;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!this._acceptsKeyInput)
                return;

            if (!Char.IsControl(e.KeyChar))
            {
                e.Handled = true;

                this.OnKeyPressed(e.KeyChar);
                this.OnAfterKeyPressed(this.CaretPos.Line, this.CaretPos.Index, e.KeyChar);
                if (this.TextInserted != null)
                    this.TextInserted(this, new TextPos(this.CaretPos.Line, this.CaretPos.Index - 1), new TextPos(this.CaretPos.Line, this.CaretPos.Index), e.KeyChar.ToString());
            }

            this.InvalidateSelection();
            this.SelectionEnd = this.SelectionStart;
        }

        protected virtual void OnKeyPressed(char c)
        {
            if (this.SelectionStart < this.SelectionEnd)
            {
                this.ReplaceText(this.SelectionStart, this.SelectionEnd, c.ToString(), true);
                this.CaretPos = new TextPos(this.SelectionStart.Line, this.SelectionStart.Index + 1);
            }
            else
            {
                TextLine tlCurrent = this._lineBuffer.GetLine(this.CaretPos.Line);
                tlCurrent.InsertText(this.CaretPos.Index, c.ToString());
                this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index + 1);
                this.MeasureLineWidth(this.CaretPos.Line, true);
                this.InvalidateLine(this.CaretPos.Line);
            }
        }

        protected virtual void OnAfterKeyPressed(int line, int index, char c)
        {
        }

        protected virtual void OnCopy()
        {
            string selText = this.GetSelectedText();
            if (selText.Length > 0)
            {
                Clipboard.SetText(selText, TextDataFormat.UnicodeText);
            }
        }

        protected virtual void OnCut()
        {
            if (this.IsTextSelected())
            {
                this.OnCopy();
                this.RemoveText(this.SelectionStart, this.SelectionEnd, true);
                this.SelectionEnd = this.SelectionStart;
                this.InvalidateSelection();
            }
        }

        protected virtual void OnPaste()
        {
            string insText = Clipboard.GetText();
            OnPaste(insText);
        }

        protected virtual void OnPaste(string insText)
        {
            if (this.IsTextSelected())
            {
                TextPos end = this.SelectionEnd;
                this.SelectionEnd = this.SelectionStart;
                this.ReplaceText(this.SelectionStart, end, insText, true);
            }
            else
            {
                this.InsertTextAtPos(this._caretPos, insText, true);
            }
        }

        public string GetSelectedText()
        {
            if (this.IsTextSelected())
            {
                if (this.SelectionStart.Line == this.SelectionEnd.Line)
                {
                    return this._lineBuffer.GetLine(this.SelectionStart.Line).GetText(this.SelectionStart.Index, this.SelectionEnd.Index - this.SelectionStart.Index);
                }
                else
                {
                    StringBuilder sbRet = new StringBuilder();
                    for (int i = this.SelectionStart.Line; i <= this.SelectionEnd.Line; i++)
                    {
                        TextLine tl = this._lineBuffer.GetLine(i);
                        if (i == this.SelectionStart.Line)
                        {
                            sbRet.Append(tl.GetText(this.SelectionStart.Index, tl.Length - this.SelectionStart.Index));
                        }
                        else if (i == this.SelectionEnd.Line)
                        {
                            sbRet.Append(Environment.NewLine + tl.GetText(0, this.SelectionEnd.Index));
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

        public int RemoveText(TextPos startPos, TextPos endPos, bool invalidate)
        {
            if (endPos <= startPos)
                return 0;

            for (int i = endPos.Line; i >= startPos.Line; i--)
            {
                if (i == startPos.Line)
                {
                    TextLine tl = this._lineBuffer.GetLine(startPos.Line);
                    int length = tl.Length - startPos.Index;
                    if (i == endPos.Line)
                        length = endPos.Index - startPos.Index;
                    tl.RemoveText(startPos.Index, length);
                }
                else if (i == endPos.Line)
                {
                    TextLine tl = this._lineBuffer.GetLine(endPos.Line);
                    if (endPos.Index >= tl.Length)
                    {
                        this._lineBuffer.DeleteLine(endPos.Line);
                    }
                    else
                    {
                        tl.RemoveText(0, endPos.Index);
                    }
                }
                else
                {
                    this._lineBuffer.DeleteLine(i);
                }
            }

            int linesInvolved = endPos.Line - startPos.Line;
            if (invalidate)
            {
                if (linesInvolved > 0)
                    this.InvalidateFromLine(startPos.Line);
                else
                    this.InvalidateLine(startPos.Line);
                this.SetAutoScrollMinSize();
            }

            if (this.TextInserted != null)
                this.TextInserted(this, startPos, startPos, "");

            this.CaretPos = startPos;
            return linesInvolved + 1;
        }

        public int ReplaceText(TextPos startPos, TextPos endPos, string text, bool invalidate)
        {
            int linesRemoved = this.RemoveText(startPos, endPos, false);
            int linesInserted = this.InsertTextAtPos(startPos, text, false);
            if (invalidate)
            {
                if (linesRemoved > 1 || linesInserted > 1)
                    this.InvalidateFromLine(startPos.Line);
                else
                    this.InvalidateLine(startPos.Line);
                this.SetAutoScrollMinSize();
            }

            if (linesInserted > linesRemoved)
                return linesInserted;
            return linesRemoved;
        }

        public int InsertTextAtPos(TextPos pos, string text, bool invalidate)
        {
            if (text == null || text.Length < 1)
                return 0;

            TextPos posStart = pos;
            
            string[] insText = text.Replace("\r", "").Split(new char[] { '\n' });
            int invalidateFrom = pos.Line;
            for (int i = 0; i < insText.Length; i++)
            {
                if (i == 0)
                {
                    TextLine tl = this._lineBuffer.GetLine(pos.Line);
                    tl.InsertText(pos.Index, insText[i]);
                    pos.Index += insText[i].Length;
                    this.MeasureLineWidth(pos.Line, true);
                }
                else
                {
                    pos.Line++;
                    TextLine tl = this._lineBuffer.InsertLine(pos.Line);
                    tl.AppendText(insText[i]);
                    pos.Index = tl.Length;
                    this.MeasureLineWidth(pos.Line, true);
                }
            }

            if (invalidate)
            {
                if (insText.Length > 1)
                    this.InvalidateFromLine(invalidateFrom);
                else
                    this.InvalidateLine(invalidateFrom);
                this.SetAutoScrollMinSize();
            }

            if (this.TextInserted != null)
                this.TextInserted(this, posStart, pos, text);

            this.CaretPos = pos;
            return insText.Length;
        }

        public bool IsTextSelected()
        {
            if (this.SelectionStart != this.SelectionEnd && this.SelectionStart < this.SelectionEnd)
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

            if (!this.IsCaretInsideScreen())
            {
                this.ScrollLines(-1);
            }
        }

        protected virtual void OnDownKey()
        {
            if (this.CaretPos.Line < (this._lineBuffer.Count - 1))
            {
                int downLineLength = _lineBuffer.GetLine(CaretPos.Line + 1).Length;
                CaretPos = new TextPos(CaretPos.Line + 1, CaretPos.Index > downLineLength ? downLineLength : CaretPos.Index);
            }
            else
                return;

            if (!this.IsCaretInsideScreen())
            {
                this.ScrollLines(1);
            }
        }

        protected virtual void Beep()
        {
            //System.Console.Beep();
        }

        protected virtual void OnLeftKey()
        {
            if (this.CaretPos.Index > 0)
            {
                this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index - 1);
            }
            else if (this.CaretPos.Line > 0)
            {
                this.CaretPos = new TextPos(this.CaretPos.Line - 1, this._lineBuffer.GetLine(this.CaretPos.Line - 1).Length);
            }
            else
            {
                this.Beep();
                return;
            }
        }

        protected virtual void OnRightKey()
        {
            if (this.CaretPos.Index < this._lineBuffer.GetLine(this.CaretPos.Line).Length)
            {
                this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index + 1);
            }
            else if (this.CaretPos.Line < (this._lineBuffer.Count - 1))
            {
                this.CaretPos = new TextPos(this.CaretPos.Line + 1, 0);
            }
            else
            {
                this.Beep();
                return;
            }
        }

        protected virtual void OnTabKey()
        {
            TextLine tlCurrent = this._lineBuffer.GetLine(this.CaretPos.Line);
            if (this._treatTabAsSpaces)
            {
                for (int i = 0; i < this._numSpacesForTab; i++)
                {
                    tlCurrent.InsertText(this.CaretPos.Index, " ");
                    this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index + 1);
                }
            }
            else
            {
                tlCurrent.InsertText(this.CaretPos.Index, "\t");
                this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index + 1);
            }
            this.MeasureLineWidth(this.CaretPos.Line, true);
            this.InvalidateLine(this.CaretPos.Line);
        }

        protected virtual void OnBackKey()
        {
            if (this.IsTextSelected())
            {
                this.ReplaceText(this.SelectionStart, this.SelectionEnd, "", true);
                this.CaretPos = this.SelectionStart;
                this.SelectionEnd = this.SelectionStart;
            }
            else
            {
                if (this.CaretPos.Index > 0)
                {
                    this.CaretPos = new TextPos(this.CaretPos.Line, this.CaretPos.Index - 1);
                    this._lineBuffer.GetLine(this.CaretPos.Line).RemoveText(this.CaretPos.Index, 1);
                    this.InvalidateLine(this.CaretPos.Line);
                }
                else if (this.CaretPos.Line > 0)
                {
                    TextLine tlRemLine = this._lineBuffer.GetLine(this.CaretPos.Line);
                    this._lineBuffer.DeleteLine(this.CaretPos.Line);
                    this.CaretPos = new TextPos(this.CaretPos.Line - 1, this._lineBuffer.GetLine(this.CaretPos.Line - 1).Length);
                    this._lineBuffer.GetLine(this.CaretPos.Line).AppendText(tlRemLine);

                    this.SetAutoScrollMinSize();
                    this.InvalidateFromLine(this.CaretPos.Line);
                    if (!this.IsCaretInsideScreen())
                        this.ScrollLines(-1);
                }
                else
                {
                    this.Beep();
                    return;
                }
            }
        }

        protected virtual void OnEnterKey()
        {
            TextLine tlCurrent = this._lineBuffer.GetLine(this.CaretPos.Line);
            if (this.CaretPos.Index < tlCurrent.Length)
            {
                TextLine tlNew = new TextLine(tlCurrent.GetText(this.CaretPos.Index, tlCurrent.Length - this.CaretPos.Index), this.ForeColor);
                List<TextLineFormating> formating = tlCurrent.GetFormating();
                foreach (TextLineFormating format in formating)
                {
                    if ((format.Index + format.Length) >= this.CaretPos.Index)
                    {
                        int diff = format.Index - this.CaretPos.Index;
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
                tlCurrent.RemoveText(this.CaretPos.Index, tlCurrent.Length - this.CaretPos.Index);
                this._lineBuffer.InsertLine(this.CaretPos.Line + 1).AppendText(tlNew);
                this.InvalidateFromLine(this.CaretPos.Line);
                this.CaretPos = new TextPos(this.CaretPos.Line + 1, 0);
            }
            else
            {
                this._lineBuffer.InsertLine(this.CaretPos.Line + 1);
                this.CaretPos = new TextPos(this.CaretPos.Line + 1, 0);
                this.InvalidateFromLine(this.CaretPos.Line);
            }

            this.UpdateLineNumWidth();
            this.SetAutoScrollMinSize();
            if (!this.IsCaretInsideScreen())
                this.ScrollToCaret();
        }
    }
}
