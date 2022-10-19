using System.Windows.Forms;

namespace SqlStudio
{
    public partial class TextOutputDialog : Form
    {
        public TextOutputDialog(string content)
        {
            InitializeComponent();
            textBoxContent.Text = content;
        }
    }
}
