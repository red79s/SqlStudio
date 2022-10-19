using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public class TextOutputTabPage : TabPage
    {
        RichTextBox _richTextBox = new RichTextBox();
        public TextOutputTabPage()
        {
            _richTextBox.Dock = DockStyle.Fill;
            Controls.Add(_richTextBox);
        }

        public void SetText(string value)
        {
            _richTextBox.Text = value;
        }
    }
}
