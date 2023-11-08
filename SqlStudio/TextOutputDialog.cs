using System;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SqlStudio
{
    public partial class TextOutputDialog : Form
    {
        public TextOutputDialog(string content)
        {
            InitializeComponent();

            textBoxContent.Text = content;
            textBoxContent.SelectionStart = 0;
            textBoxContent.SelectionLength = 0;
        }

        private void CloseButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormatButton_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxContent.Text))
            {
                return;
            }

            try
            {
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(textBoxContent.Text);

                textBoxContent.Text = JsonSerializer.Serialize(jsonElement, options);
                return;
            }
            catch (Exception)
            {
            }

            try
            {
                XDocument doc = XDocument.Parse(textBoxContent.Text);
                textBoxContent.Text = doc.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            var findDialog = new FindDialog(textBoxContent.SelectedText);
            findDialog.Owner = this;
            findDialog.StartPosition = FormStartPosition.CenterParent;

            if (findDialog.ShowDialog() == DialogResult.OK)
            {
                var index = textBoxContent.Text.IndexOf(findDialog.SearchText, textBoxContent.SelectionStart + textBoxContent.SelectionLength, StringComparison.CurrentCultureIgnoreCase);
                if (index >= 0)
                {
                    textBoxContent.SelectionStart = index;
                    textBoxContent.SelectionLength = findDialog.SearchText.Length;
                    textBoxContent.ScrollToCaret();
                    textBoxContent.Focus();
                }
                else
                {
                    MessageBox.Show(this, "No more matches found.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
