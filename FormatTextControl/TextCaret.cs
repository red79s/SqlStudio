using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace FormatTextControl
{
    class TextCaret
    {
        [DllImport("user32.dll")]
        public static extern int CreateCaret(IntPtr hwnd, IntPtr hbm, int cx, int cy);
        [DllImport("user32.dll")]
        public static extern int DestroyCaret();
        [DllImport("user32.dll")]
        public static extern int SetCaretPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern int ShowCaret(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern int HideCaret(IntPtr hwnd);

        private Control ctrl;
        private Size size;
        private Point pos;
        private bool bVisible;

        public TextCaret(Control ctrl)
        {
            this.ctrl = ctrl;
            Position = Point.Empty;
            Size = new Size(1, ctrl.Font.Height);
            Control.GotFocus += new EventHandler(OnGotFocus);
            Control.LostFocus += new EventHandler(OnLostFocus);

            if (ctrl.Focused)
                OnGotFocus(ctrl, new EventArgs());
        }

        public Control Control
        {
            get { return ctrl; }
        }

        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        public Point Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                if (this.Visible)
                {
                    SetCaretPos(pos.X, pos.Y);
                }
            }
        }

        public bool Visible
        {
            get
            {
                return bVisible;
            }
            set
            {
                bVisible = value;
                if (bVisible)
                    ShowCaret(Control.Handle);
                else
                    HideCaret(Control.Handle);
            }
        }

        public void Dispose()
        {
            if (ctrl.Focused)
                OnLostFocus(ctrl, new EventArgs());
            Control.GotFocus -= new EventHandler(OnGotFocus);
            Control.LostFocus -= new EventHandler(OnLostFocus);
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            CreateCaret(Control.Handle, IntPtr.Zero, Size.Width, Size.Height);
            SetCaretPos(Position.X, Position.Y);
            Visible = true;
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            Visible = false;
            DestroyCaret();
        }
    }
}
