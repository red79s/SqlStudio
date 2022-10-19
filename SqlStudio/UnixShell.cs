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
			this.AcceptsTab = true;
			this.Multiline = true;
			this.alLinePos = new ArrayList();
			this.Text = string.Empty;
			this.sPriProm = "$ ";
			this.sSecProm = ": ";
			
			this.history = new History();
			InsertNewPriLine();
		}

		public string PriProm
		{
			get
			{
				return this.sPriProm;
			}
			set
			{
				this.sPriProm = value;
			}
		}

		public string SecProm
		{
			get
			{
				return this.sSecProm;
			}
			set
			{
				this.sSecProm = value;
			}
		}

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
            if (!this.bAccseptInput)
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
				Debug(string.Format("OnKeyDown: {0} ({1}) cc={2}", (char)e.KeyValue, e.KeyValue, this.curCursor));
			if(iDebug>3)
			{
				for(int i=0; i<this.alLinePos.Count;i++)
				{
					int b = (int)this.alLinePos[i];
					int end = base.Text.Length;
					if(i < (this.alLinePos.Count - 1))
						end = (int)this.alLinePos[i+1];
					string lText = base.Text.Substring(b, end - b).Replace(System.Environment.NewLine,"NN");
					Debug(string.Format("line={0},b={1},e={2} line<{3}>", i, b, end, lText));
				}
			}
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
            if (!this.bAccseptInput)
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
				Debug(string.Format("OnKeyPress: {0} ({1}) cc={2} ", e.KeyChar, (int)e.KeyChar, this.curCursor));
			if(iDebug>3)
			{
				for(int i=0; i<this.alLinePos.Count;i++)
				{
					int b = (int)this.alLinePos[i];
					int end = base.Text.Length;
					if(i < (this.alLinePos.Count - 1))
						end = (int)this.alLinePos[i+1];
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
				string left = this.Text.Substring(0, this.curCursor) + lines[i];
				string right = this.Text.Substring(this.curCursor, this.Text.Length - this.curCursor);
				if(i < (lines.Length - 1)) //insert new line break
				{
					left += System.Environment.NewLine + this.sSecProm;
					int line = GetLine();
					this.alLinePos.Insert(line + 1, left.Length - this.sSecProm.Length);
				}
				this.curCursor = left.Length;
				base.Text = left + right;
			}

			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
			this.ScrollToCaret();
		}

		protected void InsertChar()
		{
			if(this.curCursor < base.Text.Length)
			{
				for(int i=GetLine()+1; i<this.alLinePos.Count; i++)
					this.alLinePos[i] = (int)this.alLinePos[i] + 1;
			}
			this.SelectionStart = this.curCursor;
			this.curCursor++;
			this.SelectionLength = 0;

			this.completerState = (int)CompleterState.UNINITALIZED;
		}

		protected void InsertChar(char e)
		{
			if(this.curCursor != this.SelectionStart)
			{
				this.SelectionStart = this.curCursor;
				this.SelectionLength = 0;
			}
			if(this.curCursor < this.Text.Length)
			{	
				string left = this.Text.Substring(0, this.curCursor);
				string right = this.Text.Substring(this.curCursor, this.Text.Length - this.curCursor);
				if(e == '\n')
				{
					int line = GetLine();
					base.Text = left + System.Environment.NewLine + right;
					this.curCursor += System.Environment.NewLine.Length;
					this.alLinePos.Insert(line + 1, this.curCursor);

					for(int i=GetLine()+1; i<this.alLinePos.Count; i++)
						this.alLinePos[i] = (int)this.alLinePos[i] + System.Environment.NewLine.Length;

					this.InsertString(this.sSecProm);
				}
				else
				{
					for(int i=GetLine()+1; i<this.alLinePos.Count; i++)
						this.alLinePos[i] = (int)this.alLinePos[i] + 1;

					base.Text = left + e + right;
					this.curCursor++;
				}
			}
			else
			{
				if(e == '\n')
				{
					base.Text += System.Environment.NewLine;
					this.curCursor = base.Text.Length;
					this.alLinePos.Add(this.curCursor);

					this.InsertString(this.sSecProm);
				}
				else
				{
					base.Text += e;
					this.curCursor++;
				}
			}
			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		protected void HandleCopy(System.Windows.Forms.KeyPressEventArgs e)
		{
			if(this.iDebug>1)
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
				if(this.iDebug>1)
					Debug("text on Clipboard: " + dText);
                
                if (this.SelectionLength > 0)
                {
                    if (this.SelectionStart < this.startCmd)
                    {
                        int diff = this.startCmd - this.SelectionStart;
                        if (diff < this.SelectionLength)
                        {
                            this.SelectionStart = this.SelectionStart + diff;
                            this.SelectionLength = this.SelectionLength - diff;
                        }
                        else
                        {
                            this.SelectionStart = this.curCursor;
                            this.SelectionLength = 0;
                        }
                    }

                    this.RemoveSelection(this.SelectionStart, this.SelectionLength);
                }
				
                this.InsertString(cText);
			}
			this.ScrollToCaret();
		}
		
        
		protected void HandleCut(System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = true;
			if(this.SelectionStart < (this.startCmd + this.sPriProm.Length))
			{
				Clipboard.SetDataObject(base.SelectedText,true);
			}
			else
			{
				string selText = GetRemoveSelection(this.SelectionStart, this.SelectionLength);
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
			this.alLinePos.RemoveRange(lineBegin + 1, linesRemoved);
			
			for(int i = (lineEnd + 1 - linesRemoved); i < this.alLinePos.Count; i++)
			{
				this.alLinePos[i] = (int)this.alLinePos[i] - len;
			}
			
			if(this.curCursor > begin && this.curCursor <= (begin + len))
				this.curCursor = begin;
			else if(this.curCursor > (begin + len))
				this.curCursor -= len;

			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		private string GetSelection(int begin, int len)
		{
			string cmd = string.Empty;
			int subBegin = begin;
			for(int i=GetLine(begin) + 1; i<=GetLine(begin + len); i++)
			{
				int lbPos = (int)this.alLinePos[i];
				if(lbPos > (begin + len))
					lbPos = begin + len;
				cmd += base.Text.Substring(subBegin, lbPos - subBegin);
				subBegin = lbPos + this.sSecProm.Length;
			}
			cmd += base.Text.Substring(subBegin, begin + len - subBegin);
			return cmd;
		}

		

		protected void HandleEnter(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(this.EndCmd())
			{
				string cmd = this.GetCmd();
				this.history.Add(cmd);

                this.bAccseptInput = false;
				
                if(CommandEvent != null)
					CommandEvent(this, cmd);
                
			}
			else
			{
				this.InsertChar('\n');
			}
		}

        public void CommandFinished()
        {
            this.bAccseptInput = true;
            this.InsertNewPriLine();
        }

		public string GetCmd()
		{
			return GetSelection(this.startCmd + this.sPriProm.Length, base.Text.Length - (this.startCmd + this.sPriProm.Length));
		}

		private string StripEndChar(string cmd)
		{
			if(this.endChar == 0)
				return cmd;

			int i=cmd.Length - 1;
			while(i>0 && cmd[i]!= this.endChar)
				i--;
			return cmd.Substring(0, i);
		}

		protected void HandleBackspace(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(this.curCursor <= (this.startCmd + this.sPriProm.Length))
				return;
			if(this.curCursor != this.SelectionStart)
			{
				this.SelectionStart = this.curCursor;
				this.SelectionLength = 0;
			}
			int line = GetLine();
			if(this.curCursor <= ((int)this.alLinePos[line] + this.sSecProm.Length))
			{
				base.Text = base.Text.Substring(0, this.curCursor - this.sSecProm.Length - System.Environment.NewLine.Length) + base.Text.Substring(this.curCursor, base.Text.Length - this.curCursor);
				this.curCursor -= (this.sSecProm.Length + System.Environment.NewLine.Length);
				this.alLinePos.RemoveAt(line);
				for(int i=line; i<this.alLinePos.Count; i++)
				{
					int newLineStart = (int)this.alLinePos[i] - (this.sSecProm.Length + System.Environment.NewLine.Length);
					this.alLinePos[i] = newLineStart;
				}
			}
			else
			{
				base.Text = base.Text.Substring(0, this.curCursor - 1) + base.Text.Substring(this.curCursor, base.Text.Length - this.curCursor);
				this.curCursor --;				
			}
			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		protected void HandleTab(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;

            if (this.CompleterEvent != null)
            {
                List<string> completions = new List<string>();
                string cmd = this.GetCmd();
                if (cmd.Length < 1)
                    return;

                this.CompleterEvent(this, cmd, cmd.Length - 1, ref completions);

                if (completions != null && completions.Count > 0)
                {
                    if (completions.Count == 1)
                    {
                        this.InsertString(completions[0].Substring(cmd.Length, completions[0].Length - cmd.Length));
                    }
                    else
                    {
                        CompleterSelect cs = new CompleterSelect();
                        cs.SetItems(completions.ToArray());
                        if (cs.ShowDialog() == DialogResult.OK)
                        {
                            this.InsertString(cs.SelectedItem.Substring(cmd.Length, cs.SelectedItem.Length - cmd.Length));
                        }
                    }
                }
            }
			
		}

		public void InsertCompletion(string partial, string[] comp)
		{
			if(comp.Length < 1)
			{
				this.completerState = (int)CompleterState.UNINITALIZED;
			}
			else if(comp.Length == 1)
			{
				this.completerState = (int)CompleterState.UNINITALIZED;
				this.InsertString(comp[0].Substring(partial.Length, comp[0].Length - partial.Length));
			}
			else if(this.completerState == (int)CompleterState.ASKING)
			{
                CompleterSelect cs = new CompleterSelect();
                cs.SetItems(comp);
                if (cs.ShowDialog() == DialogResult.OK)
                {
                    this.InsertString(cs.SelectedItem.Substring(partial.Length, cs.SelectedItem.Length - partial.Length));
                }
                
				this.completerState = (int)CompleterState.UNINITALIZED;
			}
		}

		public void ReInsertCmd(string str)
		{
			string cmd = GetCmd();
			int iCursor = this.curCursor - this.startCmd;
			this.RemoveSelection(this.startCmd, base.Text.Length - this.startCmd);
			base.Text += str;
			this.InsertNewPriLine();
			this.InsertString(cmd);
			this.curCursor = this.startCmd + iCursor;
			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		private string GetPartialCmd()
		{
			int i = this.curCursor - 1;
			while(i > (this.startCmd + this.sPriProm.Length) && base.Text[i] != ' ')
				i--;
			if(base.Text[i] == ' ')
				i++;
			return base.Text.Substring(i , this.curCursor - i);
		}

		protected void HandleArrowUpDown(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;

			if(this.history.HaveTemp())
			{
				this.RemoveSelection(this.startCmd + this.sPriProm.Length, base.Text.Length - this.startCmd - this.sPriProm.Length);
			}
			else
			{
				this.history.AddTemp(this.GetSelection(this.startCmd + this.sPriProm.Length, base.Text.Length - this.startCmd - this.sPriProm.Length));
				this.RemoveSelection(this.startCmd + this.sPriProm.Length, base.Text.Length - this.startCmd - this.sPriProm.Length);
			}
			if(e.KeyValue == (int)KeyCodes.UP_ARROW)
			{
				this.InsertString(this.history.getPrev());
			}
			else
			{
				this.InsertString(this.history.getNext());
			}
			base.ScrollToCaret();
		}

		protected void HandleArrowLeftRight(System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;
			if(this.curCursor <= (this.startCmd + this.sPriProm.Length) && e.KeyValue == (int)KeyCodes.LEFT_ARROW)
				return;
			if(this.curCursor >= base.Text.Length && e.KeyValue == (int)KeyCodes.RIGHT_ARROW)
				return;
			if(this.curCursor != this.SelectionStart)
			{
				this.SelectionStart = this.curCursor;
				this.SelectionLength = 0;
			}
			int line = GetLine();
			if(e.KeyValue == (int)KeyCodes.LEFT_ARROW)
			{
				if(this.curCursor <= ((int)this.alLinePos[line] + this.sSecProm.Length))
				{
					this.curCursor -= (this.sSecProm.Length + System.Environment.NewLine.Length);
				}
				else
				{
					this.curCursor --;				
				}
			}
			else
			{
				if(this.curCursor >= GetEndOfLine(line))
				{
					this.curCursor += (System.Environment.NewLine.Length + this.sSecProm.Length);
				}
				else
				{
					this.curCursor++;
				}
			}
			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		public void InsertNewPriLine()
		{
			if(base.Text.Length > 0)
				base.Text += System.Environment.NewLine;
			this.curCursor = base.Text.Length;
			this.startCmd = this.curCursor;
			this.alLinePos.Clear();
			this.alLinePos.Add(this.startCmd);
			InsertString(this.sPriProm);
			this.ScrollToCaret();
		}
		
		private void InsertParamLine(string sParamName)
		{
			if((this.startCmd + this.sPriProm.Length) == this.curCursor)
			{
				base.Text = base.Text.Substring(0, this.startCmd);
			}
			else if(base.Text.Length > 0)
			{
				base.Text += System.Environment.NewLine;
			}
			base.Text += sParamName;
			this.curCursor = base.Text.Length;
			this.startCmd = this.curCursor;
			this.alLinePos.Clear();
			this.alLinePos.Add(this.startCmd);
		}

		private int GetLine(int pos)
		{
			for(int i=0; i<this.alLinePos.Count; i++)
			{
				if((int)this.alLinePos[i] > pos)
					return i - 1;
			}
			return this.alLinePos.Count - 1;
		}
		
		private int GetLine()
		{
			return GetLine(this.curCursor);
		}

		private int GetEndOfLine(int line)
		{
			if(line == (this.alLinePos.Count - 1))
				return base.Text.Length;
			return base.Text.IndexOf(System.Environment.NewLine, (int)this.alLinePos[line]);
		}

		private bool EndCmd()
		{
			if((int)this.endChar == 0)
				return true;
			string exp = string.Format("{0} *$", this.endChar.ToString());
			if(Regex.IsMatch(base.Text, exp, RegexOptions.Multiline))
				return true;
			return false;
		}

		public void InsertNonCmdString(string str)
		{
			str = str.Replace("\r","");
			str = str.Replace("\n", System.Environment.NewLine);
			base.Text += str;
			this.curCursor = base.Text.Length;
			this.SelectionStart = this.curCursor;
			this.SelectionLength = 0;
		}

		public void InsertCmdString(string str)
		{
			this.InsertString(str);
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
				if(this.SelectionStart > (this.startCmd + this.sPriProm.Length))
				{
					this.curCursor = this.SelectionStart;
					for(int i=1; i<this.alLinePos.Count; i++)
					{
						if(this.curCursor >= (int)this.alLinePos[i] && this.curCursor < ((int)this.alLinePos[i] + this.sSecProm.Length))
						{
							this.curCursor += this.sSecProm.Length;
							this.SelectionStart = this.curCursor;
							break;
						}
					}
					this.SelectionLength = 0;
				}
				if(this.iDebug>1)
					Debug(string.Format("cc={0}", this.curCursor));
			}
		}

		private void Debug(string msg)
		{
			if(DebugEvent != null)
				DebugEvent(this, msg);
		}

		public void SaveHist(string sFile)
		{
			this.history.Save(sFile);
		}

		public void LoadHist(string sFile)
		{
			this.history.Load(sFile);
		}

		public int DebugLevel
		{
			set
			{
				this.iDebug = value;
			}
			get
			{
				return this.iDebug;
			}
		}
	}
}
