using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SqlStudio
{
	/// <summary>
	/// Summary description for UnixShell.
	/// </summary>
	public class UnixShell : System.Windows.Forms.TextBox
	{
		public delegate void debug(object sender, string msg);
		public event debug DebugEvent;
		public delegate void Command(object sender, string cmd);
		public event Command CommandEvent;
		public delegate void Completer(object sender, string cmd, int currentIndex, ref List<string> posibleCompletions);
		public event Completer CompleterEvent;

		private enum KeyCodes {ENTER=13, BACKSPACE=8, TAB=9, UP_ARROW=38, DOWN_ARROW=40, LEFT_ARROW=37, RIGHT_ARROW=39, SPACE=32, SHIFT=16, CTRL=17, COPY=3, PASTE=22, CUT=24};
		private enum CompleterState {UNINITALIZED=0, INITIALIZED=1, ASKING=2, ASKED=3};
		private string sPriProm = null;
		private string sSecProm = null;
		private char endChar = ';';
		private int curCursor = 0;
		private int startCmd = 0;
		private ArrayList alLinePos = null;
		private History history = null;
		private int completerState = (int)CompleterState.UNINITALIZED;
		private int iDebug = 0;
        private bool bAccseptInput = true;

		public UnixShell() : base()
		{
			AcceptsTab = true;
			Multiline = true;
			alLinePos = new ArrayList();
			Text = string.Empty;
			sPriProm = "$ ";
			sSecProm = ": ";
			
			history = new History();
			InsertNewPriLine();
		}

		public string PriProm
		{
			get
			{
				return sPriProm;
			}
			set
			{
				sPriProm = value;
			}
		}

		public string SecProm
		{
			get
			{
				return sSecProm;
			}
			set
			{
				sSecProm = value;
			}
		}

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
            if (!bAccseptInput)
            {
                e.Handled = true;
                return;
            }

			base.ScrollToCaret();
			//base.OnKeyDown (e);
			switch(e.KeyValue)
			{
				case (int)KeyCodes.ENTER:		HandleEnter(e); break;
				case (int)KeyCodes.BACKSPACE:	HandleBackspace(e); break;
				case (int)KeyCodes.TAB:			HandleTab(e); break;
				case (int)KeyCodes.UP_ARROW:	HandleArrowUpDown(e); break;
				case (int)KeyCodes.DOWN_ARROW:	HandleArrowUpDown(e); break;
				case (int)KeyCodes.LEFT_ARROW:	HandleArrowLeftRight(e); break;
				case (int)KeyCodes.RIGHT_ARROW: HandleArrowLeftRight(e); break;
			}
			if(iDebug>2)
				Debug(string.Format("OnKeyDown: {0} ({1}) cc={2}", (char)e.KeyValue, e.KeyValue, curCursor));
			if(iDebug>3)
			{
				for(int i=0; i<alLinePos.Count;i++)
				{
					int b = (int)alLinePos[i];
					int end = base.Text.Length;
					if(i < (alLinePos.Count - 1))
						end = (int)alLinePos[i+1];
					string lText = base.Text.Substring(b, end - b).Replace(System.Environment.NewLine,"NN");
					Debug(string.Format("line={0},b={1},e={2} line<{3}>", i, b, end, lText));
				}
			}
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
            if (!bAccseptInput)
            {
                e.Handled = true;
                return;
            }

			base.ScrollToCaret();
			//base.OnKeyPress (e);
			switch((int)e.KeyChar)
			{
				case (int)KeyCodes.TAB:			e.Handled = true; break;
				case (int)KeyCodes.ENTER:		e.Handled = true; break;
				case (int)KeyCodes.BACKSPACE:	e.Handled = true; break;
				case (int)KeyCodes.COPY:		HandleCopy(e); break;
				case (int)KeyCodes.PASTE:		HandlePaste(e); break;
				case (int)KeyCodes.CUT:			HandleCut(e); break;
				default: InsertChar(); break;
			}
			if(iDebug>2)
				Debug(string.Format("OnKeyPress: {0} ({1}) cc={2} ", e.KeyChar, (int)e.KeyChar, curCursor));
			if(iDebug>3)
			{
				for(int i=0; i<alLinePos.Count;i++)
				{
					int b = (int)alLinePos[i];
					int end = base.Text.Length;
					if(i < (alLinePos.Count - 1))
						end = (int)alLinePos[i+1];
					string lText = base.Text.Substring(b, end - b).Replace(System.Environment.NewLine,"NN");
					Debug(string.Format("line={0},b={1},e={2} line<{3}>", i, b, end, lText));
				}
			}
		}
		
		protected void InsertString(string str)
		{
			str = str.Replace("\r","");
			string[] lines = str.Split('\n');
			for(int i = 0; i < lines.Length; i++) //insert each line
			{
				string left = Text.Substring(0, curCursor) + lines[i];
				string right = Text.Substring(curCursor, Text.Length - curCursor);
				if(i < (lines.Length - 1)) //insert new line break
				{
					left += System.Environment.NewLine + sSecProm;
					int line = GetLine();
					alLinePos.Insert(line + 1, left.Length - sSecProm.Length);
				}
				curCursor = left.Length;
				base.Text = left + right;
			}

			SelectionStart = curCursor;
			SelectionLength = 0;
			ScrollToCaret();
		}

		protected void InsertChar()
		{
			if(curCursor < base.Text.Length)
			{
				for(int i=GetLine()+1; i<alLinePos.Count; i++)
					alLinePos[i] = (int)alLinePos[i] + 1;
			}
			SelectionStart = curCursor;
			curCursor++;
			SelectionLength = 0;

			completerState = (int)CompleterState.UNINITALIZED;
		}

		protected void InsertChar(char e)
		{
			if(curCursor != SelectionStart)
			{
				SelectionStart = curCursor;
				SelectionLength = 0;
			}
			if(curCursor < Text.Length)
			{	
				string left = Text.Substring(0, curCursor);
				string right = Text.Substring(curCursor, Text.Length - curCursor);
				if(e == '\n')
				{
					int line = GetLine();
					base.Text = left + System.Environment.NewLine + right;
					curCursor += System.Environment.NewLine.Length;
					alLinePos.Insert(line + 1, curCursor);

					for(int i=GetLine()+1; i<alLinePos.Count; i++)
						alLinePos[i] = (int)alLinePos[i] + System.Environment.NewLine.Length;

					InsertString(sSecProm);
				}
				else
				{
					for(int i=GetLine()+1; i<alLinePos.Count; i++)
						alLinePos[i] = (int)alLinePos[i] + 1;

					base.Text = left + e + right;
					curCursor++;
				}
			}
			else
			{
				if(e == '\n')
				{
					base.Text += System.Environment.NewLine;
					curCursor = base.Text.Length;
					alLinePos.Add(curCursor);

					InsertString(sSecProm);
				}
				else
				{
					base.Text += e;
					curCursor++;
				}
			}
			SelectionStart = curCursor;
			SelectionLength = 0;
		}

		protected void HandleCopy(System.Windows.Forms.KeyPressEventArgs e)
		{
			if(iDebug>1)
				Debug("selected text <" + base.SelectedText + ">");

			Clipboard.SetDataObject(base.SelectedText, true);
		}
		
		protected void HandlePaste(System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = true;
			if(Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
			{
				string cText = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
				string dText = cText.Replace("\r","R");
				dText = dText.Replace("\n","N");
				if(iDebug>1)
					Debug("text on Clipboard: " + dText);
                
                if (SelectionLength > 0)
                {
                    if (SelectionStart < startCmd)
                    {
                        int diff = startCmd - SelectionStart;
                        if (diff < SelectionLength)
                        {
                            SelectionStart = SelectionStart + diff;
                            SelectionLength = SelectionLength - diff;
                        }
                        else
                        {
                            SelectionStart = curCursor;
                            SelectionLength = 0;
                        }
                    }

                    RemoveSelection(SelectionStart, SelectionLength);
                }
				
                InsertString(cText);
			}
			ScrollToCaret();
		}
		
        
		protected void HandleCut(System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = true;
			if(SelectionStart < (startCmd + sPriProm.Length))
			{
				Clipboard.SetDataObject(base.SelectedText,true);
			}
			else
			{
				string selText = GetRemoveSelection(SelectionStart, SelectionLength);
				Clipboard.SetDataObject(selText,true);
			}
		}

		private string GetRemoveSelection(int begin, int len)
		{
			string ret = GetSelection(begin, len);
			RemoveSelection(begin, len);
			return ret;
		}

		private void RemoveSelection(int begin, int len)
		{
			if(begin > base.Text.Length)
				return;
			if(begin + len > base.Text.Length)
				len = base.Text.Length - begin;
			
			int lineBegin = GetLine(begin);
			int lineEnd = GetLine(begin + len);
			base.Text = base.Text.Substring(0, begin) + base.Text.Substring(begin + len, base.Text.Length - (begin + len));
			
			int linesRemoved = lineEnd - lineBegin;
			alLinePos.RemoveRange(lineBegin + 1, linesRemoved);
			
			for(int i = (lineEnd + 1 - linesRemoved); i < alLinePos.Count; i++)
			{
				alLinePos[i] = (int)alLinePos[i] - len;
			}
			
			if(curCursor > begin && curCursor <= (begin + len))
				curCursor = begin;
			else if(curCursor > (begin + len))
				curCursor -= len;

			SelectionStart = curCursor;
			SelectionLength = 0;
		}

		private string GetSelection(int begin, int len)
		{
			string cmd = string.Empty;
			int subBegin = begin;
			for(int i=GetLine(begin) + 1; i<=GetLine(begin + len); i++)
			{
				int lbPos = (int)alLinePos[i];
				if(lbPos > (begin + len))
					lbPos = begin + len;
				cmd += base.Text.Substring(subBegin, lbPos - subBegin);
				subBegin = lbPos + sSecProm.Length;
			}
			cmd += base.Text.Substring(subBegin, begin + len - subBegin);
			return cmd;
		}

		

		protected void HandleEnter(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(EndCmd())
			{
				string cmd = GetCmd();
				history.Add(cmd);

                bAccseptInput = false;
				
                if(CommandEvent != null)
					CommandEvent(this, cmd);
                
			}
			else
			{
				InsertChar('\n');
			}
		}

        public void CommandFinished()
        {
            bAccseptInput = true;
            InsertNewPriLine();
        }

		public string GetCmd()
		{
			return GetSelection(startCmd + sPriProm.Length, base.Text.Length - (startCmd + sPriProm.Length));
		}

		private string StripEndChar(string cmd)
		{
			if(endChar == 0)
				return cmd;

			int i=cmd.Length - 1;
			while(i>0 && cmd[i]!= endChar)
				i--;
			return cmd.Substring(0, i);
		}

		protected void HandleBackspace(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(curCursor <= (startCmd + sPriProm.Length))
				return;
			if(curCursor != SelectionStart)
			{
				SelectionStart = curCursor;
				SelectionLength = 0;
			}
			int line = GetLine();
			if(curCursor <= ((int)alLinePos[line] + sSecProm.Length))
			{
				base.Text = base.Text.Substring(0, curCursor - sSecProm.Length - System.Environment.NewLine.Length) + base.Text.Substring(curCursor, base.Text.Length - curCursor);
				curCursor -= (sSecProm.Length + System.Environment.NewLine.Length);
				alLinePos.RemoveAt(line);
				for(int i=line; i<alLinePos.Count; i++)
				{
					int newLineStart = (int)alLinePos[i] - (sSecProm.Length + System.Environment.NewLine.Length);
					alLinePos[i] = newLineStart;
				}
			}
			else
			{
				base.Text = base.Text.Substring(0, curCursor - 1) + base.Text.Substring(curCursor, base.Text.Length - curCursor);
				curCursor --;				
			}
			SelectionStart = curCursor;
			SelectionLength = 0;
		}

		protected void HandleTab(KeyEventArgs e)
		{
			e.Handled = true;

            if (CompleterEvent != null)
            {
                List<string> completions = new List<string>();
                string cmd = GetCmd();
                if (cmd.Length < 1)
                    return;

                CompleterEvent(this, cmd, cmd.Length - 1, ref completions);

                if (completions != null && completions.Count > 0)
                {
                    if (completions.Count == 1)
                    {
                        InsertString(completions[0].Substring(cmd.Length, completions[0].Length - cmd.Length));
                    }
                    else
                    {
                        CompleterSelect cs = new CompleterSelect();
                        cs.SetItems(completions.ToArray());
                        if (cs.ShowDialog() == DialogResult.OK)
                        {
                            InsertString(cs.SelectedItem.Substring(cmd.Length, cs.SelectedItem.Length - cmd.Length));
                        }
                    }
                }
            }
			
		}

		protected void HandleArrowUpDown(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;

			if(history.HaveTemp())
			{
				RemoveSelection(startCmd + sPriProm.Length, base.Text.Length - startCmd - sPriProm.Length);
			}
			else
			{
				history.AddTemp(GetSelection(startCmd + sPriProm.Length, base.Text.Length - startCmd - sPriProm.Length));
				RemoveSelection(startCmd + sPriProm.Length, base.Text.Length - startCmd - sPriProm.Length);
			}
			if(e.KeyValue == (int)KeyCodes.UP_ARROW)
			{
				InsertString(history.getPrev());
			}
			else
			{
				InsertString(history.getNext());
			}
			base.ScrollToCaret();
		}

		protected void HandleArrowLeftRight(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(curCursor <= (startCmd + sPriProm.Length) && e.KeyValue == (int)KeyCodes.LEFT_ARROW)
				return;
			if(curCursor >= base.Text.Length && e.KeyValue == (int)KeyCodes.RIGHT_ARROW)
				return;
			if(curCursor != SelectionStart)
			{
				SelectionStart = curCursor;
				SelectionLength = 0;
			}
			int line = GetLine();
			if(e.KeyValue == (int)KeyCodes.LEFT_ARROW)
			{
				if(curCursor <= ((int)alLinePos[line] + sSecProm.Length))
				{
					curCursor -= (sSecProm.Length + System.Environment.NewLine.Length);
				}
				else
				{
					curCursor --;				
				}
			}
			else
			{
				if(curCursor >= GetEndOfLine(line))
				{
					curCursor += (System.Environment.NewLine.Length + sSecProm.Length);
				}
				else
				{
					curCursor++;
				}
			}
			SelectionStart = curCursor;
			SelectionLength = 0;
		}

		public void InsertNewPriLine()
		{
			if(base.Text.Length > 0)
				base.Text += System.Environment.NewLine;
			curCursor = base.Text.Length;
			startCmd = curCursor;
			alLinePos.Clear();
			alLinePos.Add(startCmd);
			InsertString(sPriProm);
			ScrollToCaret();
		}
		
		private void InsertParamLine(string sParamName)
		{
			if((startCmd + sPriProm.Length) == curCursor)
			{
				base.Text = base.Text.Substring(0, startCmd);
			}
			else if(base.Text.Length > 0)
			{
				base.Text += System.Environment.NewLine;
			}
			base.Text += sParamName;
			curCursor = base.Text.Length;
			startCmd = curCursor;
			alLinePos.Clear();
			alLinePos.Add(startCmd);
		}

		private int GetLine(int pos)
		{
			for(int i=0; i<alLinePos.Count; i++)
			{
				if((int)alLinePos[i] > pos)
					return i - 1;
			}
			return alLinePos.Count - 1;
		}
		
		private int GetLine()
		{
			return GetLine(curCursor);
		}

		private int GetEndOfLine(int line)
		{
			if(line == (alLinePos.Count - 1))
				return base.Text.Length;
			return base.Text.IndexOf(System.Environment.NewLine, (int)alLinePos[line]);
		}

		private bool EndCmd()
		{
			if((int)endChar == 0)
				return true;
			string exp = string.Format("{0} *$", endChar.ToString());
			if(Regex.IsMatch(base.Text, exp, RegexOptions.Multiline))
				return true;
			return false;
		}

		public void InsertNonCmdString(string str)
		{
			str = str.Replace("\r","");
			str = str.Replace("\n", System.Environment.NewLine);
			base.Text += str;
			curCursor = base.Text.Length;
			SelectionStart = curCursor;
			SelectionLength = 0;
		}

		public void InsertCmdString(string str)
		{
			InsertString(str);
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				
			}
		}

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown (e);
			if(e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				if(SelectionStart > (startCmd + sPriProm.Length))
				{
					curCursor = SelectionStart;
					for(int i=1; i<alLinePos.Count; i++)
					{
						if(curCursor >= (int)alLinePos[i] && curCursor < ((int)alLinePos[i] + sSecProm.Length))
						{
							curCursor += sSecProm.Length;
							SelectionStart = curCursor;
							break;
						}
					}
					SelectionLength = 0;
				}
				if(iDebug>1)
					Debug(string.Format("cc={0}", curCursor));
			}
		}

		private void Debug(string msg)
		{
			if(DebugEvent != null)
				DebugEvent(this, msg);
		}

		public void SaveHist(string sFile)
		{
			history.Save(sFile);
		}

		public void LoadHist(string sFile)
		{
			history.Load(sFile);
		}

		public int DebugLevel
		{
			set
			{
				iDebug = value;
			}
			get
			{
				return iDebug;
			}
		}
	}
}
