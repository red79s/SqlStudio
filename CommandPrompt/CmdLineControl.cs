using Common;
using FormatTextControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace CommandPrompt
{
    public class CmdLineControl : FormatTextControl.FormatTextControl
    {
        public delegate void CommandReadyDelegate(object sender, string cmd);
        public event CommandReadyDelegate CommandReady;
        public delegate CommandCompletionResult CommandCompletionDelegate(object sender, string cmd, int index);
        public event CommandCompletionDelegate CommandCompletion;

        private enum CmdMode { LINE, COMMAND };
        private string _cmdPrompt = "$ ";
        private TextPos _cmdStartPos;
        private TextPos _cmdInputCaret;
        private CmdMode _cmdMode = CmdMode.COMMAND;

        private CommandHistory _cmdHistory = null;
        private CommandHistory _lineHistory = null;
        private ICommandHistoryStore _commandHistoryStore;

        public CmdLineControl()
        {
            AcceptsKeyInput = false;
        }

        public void SetHistoryItems(ICommandHistoryStore commandHistoryStore)
        {
            _commandHistoryStore = commandHistoryStore;
            _cmdHistory = new CommandHistory(commandHistoryStore);
            _lineHistory = new CommandHistory(commandHistoryStore);
        }

        public List<string> GetHistoryItems()
        {
            return _cmdHistory.GetHistoryItems();
        }

        public void ClearHistoryItems()
        {
            _cmdHistory.Clear();
        }

        public void GetCommand()
        {
            GetCommand(_cmdPrompt);
        }

        public void GetCommand(string prompt)
        {
            InsertNewPrompt(prompt);
            AcceptsKeyInput = true;
            _cmdMode = CmdMode.COMMAND;
        }

        public void ExecuteCommand(string cmd)
        {
            ReplaceText(_cmdStartPos, GetTextEnd(), cmd, true);
            _cmdMode = CmdMode.LINE;
            OnEnterKey();
        }

        public void GetCommandLine(string prompt)
        {
            InsertNewPrompt(prompt);
            AcceptsKeyInput = true;
            _cmdMode = CmdMode.LINE;
        }

        public void InsertCommandOutput(string text)
        {
            AppendText(text);
            ScrollToCaret();
        }

        private void InsertNewPrompt(string prompt)
        {
            int newLineAt = 0;
            if (LineCount > 1 || GetLineLength(0) > 0)
            {
                newLineAt = LineCount;
                InsertLine(newLineAt, prompt);
            }
            else
            {
                SetText(0, prompt);
            }
            _cmdStartPos = new TextPos(newLineAt, GetLineLength(newLineAt));
            _cmdInputCaret = _cmdStartPos;
            CaretPos = _cmdStartPos;
            ScrollToCaret();
        }

        public override string Text
        {
            get
            {
                return "";
            }
        }

        protected override void OnEnterKey()
        {
            if (_cmdMode == CmdMode.LINE)
            {
                AcceptsKeyInput = false;
                string cmd = GetText(_cmdStartPos, GetTextEnd());
                if (CommandReady != null)
                    CommandReady(this, cmd);
                _lineHistory.Add(cmd);
                return;
            }
            else if (_cmdMode == CmdMode.COMMAND)
            {
                string cmd = GetText(_cmdStartPos, GetTextEnd());
                if (cmd.IndexOf(";") >= 0)
                {
                    AcceptsKeyInput = false;
                    if (CommandReady != null)
                        CommandReady(this, cmd);
                    _cmdHistory.Add(cmd);
                    return;
                }
            }
            base.OnEnterKey();
        }

        protected override bool OnBeforeCaretMove(TextPos oldPos, TextPos newPos)
        {
            if (newPos >= _cmdStartPos)
                _cmdInputCaret = newPos;

            return true;
        }

        public event EventHandler<InsertSnipitEventArgs> InsertSnipit;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                var dateString = string.Format("'{0}'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                OnPaste(dateString);
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                var picker = new DateAndTimePicker();
                picker.StartPosition = FormStartPosition.Manual;
                Point p = GetCaretLocation();
                picker.Location = GetGlobalPosition(p.X, p.Y + LineHeight);
                var selectedText = GetSelectedText();
                var selectionStart = SelectionStart;
                var selectionEnd = SelectionEnd;

                if (!string.IsNullOrEmpty(selectedText))
                {
                    if (selectedText[0] == '\'' || selectedText[0] == '"')
                        selectedText = selectedText.Substring(1, selectedText.Length - 2);
                    if (DateTime.TryParseExact(selectedText, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var selectedDateTime))
                    {
                        picker.DateTime = selectedDateTime;
                    }
                }

                if (picker.ShowDialog(this) == DialogResult.OK)
                {
                    var dateString = string.Format("{0}", picker.DateTime.ToString("yyyy-MM-dd HH:mm:00", CultureInfo.InvariantCulture));

                    SelectionStart = selectionStart;
                    SelectionEnd = selectionEnd;
                    OnPaste(selectionStart == selectionEnd ? $"'{dateString}'" : dateString);
                }
            }
            else if (e.Control)
            {
                var snipitArgs = new InsertSnipitEventArgs() { Args = e };
                InsertSnipit?.Invoke(this, snipitArgs);
                if (snipitArgs.Handled && snipitArgs.InsertText != null)
                {
                    OnPaste(snipitArgs.InsertText);
                }
                else if (!snipitArgs.Handled)
                {
                    base.OnKeyDown(e);
                }
            }
            else
                base.OnKeyDown(e);
        }

        protected override void OnKeyPressed(char c)
        {
            if (CaretPos != _cmdInputCaret)
                CaretPos = _cmdInputCaret;

            if (SelectionStart < SelectionEnd && SelectionStart < _cmdStartPos)
                return;

            base.OnKeyPressed(c);
        }

        protected override void OnLeftKey()
        {
            if (CaretPos != _cmdInputCaret)
                CaretPos = _cmdInputCaret;

            if (CaretPos > _cmdStartPos)
                base.OnLeftKey();
            else
                Beep();
        }

        protected override void OnRightKey()
        {
            if (CaretPos != _cmdInputCaret)
                CaretPos = _cmdInputCaret;

            base.OnRightKey();
        }

        protected override void OnBackKey()
        {
            if (SelectionStart < _cmdStartPos && SelectionEnd > SelectionStart)
                SelectionStart = _cmdStartPos;

            if (CaretPos != _cmdInputCaret)
                CaretPos = _cmdInputCaret;

            if (CaretPos > _cmdStartPos)
                base.OnBackKey();
            else
                Beep();
        }

        protected override void OnUpKey()
        {
            if (_cmdMode == CmdMode.LINE)
            {
                if (_lineHistory.NeedsReset)
                {
                    string cmd = GetText(_cmdStartPos, GetTextEnd());
                    _lineHistory.Reset(cmd);
                }
                string histItem = _lineHistory.GetPrev();
                if (histItem != "")
                {
                    ReplaceText(_cmdStartPos, GetTextEnd(), histItem, true);
                    ScrollToCaret();
                }
            }
            else if (_cmdMode == CmdMode.COMMAND)
            {
                if (_cmdHistory.NeedsReset)
                {
                    string cmd = GetText(_cmdStartPos, GetTextEnd());
                    _cmdHistory.Reset(cmd);
                }
                string histItem = _cmdHistory.GetPrev();
                if (histItem != "")
                {
                    ReplaceText(_cmdStartPos, GetTextEnd(), histItem, true);
                    ScrollToCaret();
                }
            }
        }

        protected override void OnDownKey()
        {
            if (_cmdMode == CmdMode.LINE)
            {
                if (_lineHistory.NeedsReset)
                {
                    return;
                }
                string histItem = _lineHistory.GetNext();
                ReplaceText(_cmdStartPos, GetTextEnd(), histItem, true);
                ScrollToCaret();
            }
            else if (_cmdMode == CmdMode.COMMAND)
            {
                if (_cmdHistory.NeedsReset)
                {
                    return;
                }
                string histItem = _cmdHistory.GetNext();
                ReplaceText(_cmdStartPos, GetTextEnd(), histItem, true);
                ScrollToCaret();
            }
        }

        private Point GetParentFormLocation()
        {
            Point pRet = new Point(0, 0);
            Control ctrl = Parent;
            while (true)
            {
                pRet = new Point(pRet.X + ctrl.Location.X + (ctrl.Size.Width - ctrl.ClientRectangle.Width), 
                                 pRet.Y + ctrl.Location.Y + (ctrl.Size.Height - ctrl.ClientRectangle.Height));
                if (ctrl.Parent == null)
                    return pRet;
                ctrl = ctrl.Parent;
            }
        }

        private Point GetGlobalPosition(int x, int y)
        {
            Point p = PointToClient(new Point(0, 0));
            return new Point(x - p.X, y - p.Y);
        }

        protected override void OnTabKey()
        {
            if (CommandCompletion != null)
            {
                TextPos tpEnd = GetTextEnd();
                int index = 0;
                string cmd = GetText(_cmdStartPos, tpEnd);
                for (int i = _cmdStartPos.Line; i < CaretPos.Line; i++)
                {
                    index += GetLineLength(i) + Environment.NewLine.Length;
                    if (i == _cmdStartPos.Line)
                        index -= _cmdStartPos.Index;
                }
                if (CaretPos.Line > _cmdStartPos.Line)
                    index += (CaretPos.Index); //point to char in front of caret
                else
                    index += (CaretPos.Index) - _cmdStartPos.Index;

                if (index < 0)
                    return;

                string selectedItem = "";
                var completetionsRes = CommandCompletion(this, cmd, index);
                if (completetionsRes.PossibleCompletions.Count > 1)
                {
                    CompleterSelect cs = new CompleterSelect();
                    cs.Text = "";
                    cs.ControlBox = false;  
                    cs.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    Point p = GetCaretLocation();
                    cs.Location = GetGlobalPosition(p.X, p.Y + LineHeight);
                    cs.SetItems(completetionsRes.PossibleCompletions.ToArray());
                    if (cs.ShowDialog() == DialogResult.OK)
                    {
                        selectedItem = cs.SelectedItem;
                    }
                }
                else if (completetionsRes.PossibleCompletions.Count > 0)
                {
                    selectedItem = completetionsRes.PossibleCompletions[0];
                }

                if (selectedItem.Length > 0)
                {
                    int selIndex = completetionsRes.CompletedTextStartIndex;
                    if (selIndex < 0)
                    {
                        AppendText(selectedItem);
                    }
                    else if (selIndex >= 0)
                    {
                        ReplaceText(new TextPos(CaretPos.Line, CaretPos.Index - (index - selIndex)), CaretPos, selectedItem, true);
                    }
                }
            }
        }
    }
}
