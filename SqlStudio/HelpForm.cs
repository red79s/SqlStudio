using System;
using System.Windows.Forms;

namespace SqlStudio
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
            webBrowser1.Url = new Uri("file://" + Application.StartupPath + @"\help.htm");
        }
    }
}