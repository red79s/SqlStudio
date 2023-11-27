using System;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class FieldUserControlDefault : UserControl, IFieldUserControl
    {
        public FieldUserControlDefault(FieldInfo fieldInfo)
        {
            InitializeComponent();
            FieldLabel.Text = fieldInfo.Name;
            Value = fieldInfo.Value;
            cbDbNull.CheckedChanged += CbDbNull_CheckedChanged;
        }

        private void CbDbNull_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDbNull.Checked)
            {
                tbValue.Enabled = false;
            }
            else
            {
                tbValue.Enabled = true;
            }
        }

        public string FieldName { get => FieldLabel.Text; set => FieldLabel.Text = value; }
        public object Value
        {
            get
            {
                if (cbDbNull.Checked)
                {
                    return DBNull.Value;
                }

                return tbValue.Text;
            }
            set
            {
                if (value == DBNull.Value || value == null)
                {
                    tbValue.Text = "NULL";
                    tbValue.Enabled = false;
                    cbDbNull.Checked = true;
                }
                else
                {
                    tbValue.Text = value.ToString();
                    tbValue.Enabled = true;
                    cbDbNull.Checked = false;
                }
            }
        }
    }
}
