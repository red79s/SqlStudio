using System;
using System.Drawing;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class FieldUserControlDateTime : FieldUserControlBase
    {
        private DateTimePicker _valueControl;
        public FieldUserControlDateTime(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            InitializeComponent();
            
            Value = fieldInfo.Value;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            _valueControl = new DateTimePicker();
            _groupBox.Controls.Add(_valueControl);
        }

        protected override void ResizeControls()
        {
            base.ResizeControls();

            _valueControl.Height = Height - 11;
            _valueControl.Width = Width - 75;
            _valueControl.Location = new Point(10, 14);
        }

        protected override void DbNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_dbNullcheckBox.Checked)
            {
                _valueControl.Value = DateTime.Now;
                _valueControl.Enabled = true;
            }
            else
            {
                _valueControl.Enabled = false;
            }
        }
        

        public override object Value
        {
            get
            {
                if (_dbNullcheckBox.Checked)
                {
                    return DBNull.Value;
                }
                return _valueControl.Value;
            }
            set
            {
                if (value == DBNull.Value)
                {
                    _valueControl.Enabled = false;
                    _dbNullcheckBox.Checked = true;
                }
                else
                {
                    _valueControl.Value = (DateTime)value;
                    _dbNullcheckBox.Checked = false;
                }
            }
        }
    }
}
