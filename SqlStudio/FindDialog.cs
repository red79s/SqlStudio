using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class FindDialog : Form
    {
        public FindDialog(string searchText = "")
        {
            InitializeComponent();
            tbSearchText.Text = searchText;
        }

        public string SearchText
        {
            get { return tbSearchText.Text; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
