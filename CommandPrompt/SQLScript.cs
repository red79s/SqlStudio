using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CommandPrompt
{
    public partial class SQLScript : UserControl
    {
        private string _fileName = null;
        public string FileName { get { return _fileName; } }

        public SQLScript()
        {
            InitializeComponent();
            var miRepeat = new ToolStripMenuItem("Expand text");
            miRepeat.Click += new EventHandler(miRepeat_Click);
            formatTextControl.ContextMenuStrip.Items.Add(miRepeat);
        }

        void miRepeat_Click(object sender, EventArgs e)
        {
            string selectedText = formatTextControl.GetSelectedText();
            if (string.IsNullOrEmpty(selectedText))
                selectedText = Clipboard.GetText();
            if (!string.IsNullOrEmpty(selectedText))
            {
                ExpandText expand = new ExpandText();
                expand.InputText = selectedText;
                if (expand.ShowDialog() == DialogResult.OK)
                {
                    string output = expand.GetExpandedText();
                    Clipboard.SetText(output);
                }
            }
            else
                MessageBox.Show("No text selected, and no text on clipboard", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public bool DataChanged
        {
            get { return true; }
        }

        public void Save()
        {
            if (_fileName != null)
                File.WriteAllLines(_fileName, formatTextControl.Lines);
        }

        public void Save(string fileName)
        {
            File.WriteAllLines(fileName, formatTextControl.Lines);
            _fileName = fileName;
        }

        public void Open(string fileName)
        {
            formatTextControl.Text = File.ReadAllText(fileName);
            _fileName = fileName;
        }

        public void Open(List<string> commands)
        {
            var commandsText = "";
            foreach (var cmd in commands)
            {
                commandsText += cmd + Environment.NewLine;
            }
            formatTextControl.Text = commandsText;
        }

        public List<string> GetQueries()
        {
            List<string> ret = new List<string>();
            string content = null;

            if (formatTextControl.SelectionEnd > formatTextControl.SelectionStart)
                content = formatTextControl.GetSelectedText();
            else
                content = formatTextControl.Text;

            string[] commands = content.Split(new char[] { ';' });
            foreach (string command in commands)
            {
                if (command.Trim() != "")
                    ret.Add(command + ";");
            }

            return ret;
        }

        public string GetSelectedText()
        {
            if (formatTextControl.SelectionEnd > formatTextControl.SelectionStart)
                return formatTextControl.GetSelectedText();
            else
                return formatTextControl.Text;
        }
    }
}
