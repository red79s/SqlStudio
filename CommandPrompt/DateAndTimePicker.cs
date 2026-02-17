using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CommandPrompt
{
    public partial class DateAndTimePicker : Form
    {
        public DateAndTimePicker()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime DateTime
        {
            get { return dateTimePicker1.Value; }
            set { dateTimePicker1.Value = value; }
        }

        private void oKbutton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
