using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class SqlFormatDialog : Form
    {
        public SqlFormatDialog()
        {
            InitializeComponent();
        }

        public string FormatedSql
        {
            get
            {
                return textBoxSql.Text;
            }
        }

        private void buttonFormat_Click(object sender, EventArgs e)
        {
            var formater = new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter();
            var formatingManager = new PoorMansTSqlFormatterLib.SqlFormattingManager(formater);

            var output = formatingManager.Format(textBoxSql.Text);

            textBoxSql.Text = output;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonSubst_Click(object sender, EventArgs e)
        {
            StringBuilder substitutedText = new StringBuilder();
            int lastIndex = 0;
            Dictionary<string, string> resolvedVariables = new Dictionary<string, string>();
            Regex regex = new Regex(@"@[0-9a-zA-Z_]+");
            var matches = regex.Matches(textBoxSql.Text);
            foreach (Match match in matches)
            {
                string substText = "";
                if (resolvedVariables.ContainsKey(match.Value))
                {
                    substText = resolvedVariables[match.Value];
                }
                else
                {
                    ParameterValueDialog pvd = new ParameterValueDialog();
                    pvd.Text = match.Value;
                    if (pvd.ShowDialog() == DialogResult.OK)
                    {
                        substText = pvd.Value;
                    }
                    else
                    {
                        substText = match.Value;
                    }

                    resolvedVariables.Add(match.Value, substText);
                }

                substitutedText.Append(textBoxSql.Text.Substring(lastIndex, match.Index - lastIndex));
                substitutedText.Append(substText);
                lastIndex = match.Index + match.Length;
            }

            textBoxSql.Text = substitutedText.ToString();
        }
    }
}
