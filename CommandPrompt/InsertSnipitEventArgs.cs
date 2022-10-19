using System;
using System.Windows.Forms;

namespace CommandPrompt
{
    public class InsertSnipitEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public string InsertText { get; set; }
        public KeyEventArgs Args { get; set; }
    }
}