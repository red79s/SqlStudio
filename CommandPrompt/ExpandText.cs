using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CommandPrompt
{
    public partial class ExpandText : Form
    {
        public ExpandText()
        {
            InitializeComponent();

            txtInput.TextChanged += new EventHandler(txtInpu_TextChanged);
            txtNumExpand.TextChanged += new EventHandler(txtNumExpand_TextChanged);
            checkBoxAppendNewline.CheckedChanged += new EventHandler(checkBoxAppendNewline_CheckedChanged);
        }

        void checkBoxAppendNewline_CheckedChanged(object sender, EventArgs e)
        {
            txtPreview.Text = GetExpandedText();
        }

        private Regex _numRegex = new Regex("^[0-9]{0,5}$");
        private string _lastNumExpand = "2";
        void txtNumExpand_TextChanged(object sender, EventArgs e)
        {
            if (_numRegex.IsMatch(txtNumExpand.Text))
            {
                _lastNumExpand = txtNumExpand.Text;
            }
            else
            {
                txtNumExpand.Text = _lastNumExpand;
            }

            txtPreview.Text = GetExpandedText();
        }

        private int NumExpand
        {
            get
            {
                if (txtNumExpand.Text == "")
                    return 0;
                return int.Parse(txtNumExpand.Text);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string InputText
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        void txtInpu_TextChanged(object sender, EventArgs e)
        {
            txtPreview.Text = GetExpandedText();
        }

        public string GetExpandedText()
        {
            string input = txtInput.Text;
            int num = NumExpand;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < num; i++)
            {
                string output = "";
                try
                {
                    output = string.Format(input, i);
                }
                catch(Exception)
                {
                    output = input;
                }
                
                sb.Append(output);
                if (checkBoxAppendNewline.Checked)
                    sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
