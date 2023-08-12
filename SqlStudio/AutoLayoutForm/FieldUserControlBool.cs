using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    internal class FieldUserControlBool : FieldUserControlBase
    { 
        private CheckBox _valueControl;
        private ErrorProvider _errorProvider;

        public FieldUserControlBool(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            Value = fieldInfo.Value;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            _valueControl = new CheckBox { Appearance = Appearance.Button };
            _groupBox.Controls.Add(_valueControl);

            _errorProvider = new ErrorProvider();
            _errorProvider.SetIconAlignment(_valueControl, ErrorIconAlignment.MiddleRight);
            _errorProvider.SetIconPadding(_valueControl, 2);
            _errorProvider.BlinkRate = 1000;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;

            _valueControl.Validating += _valueControl_Validating;
        }

        private Regex _validateRegex = new Regex("[0-9. ]+");
        private void _valueControl_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

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
                _valueControl.Checked = false;
                _valueControl.Enabled = true;
            }
            else
            {
                _valueControl.Checked = false;
                _valueControl.Enabled = false;
            }
        }

        private bool GetValue(bool value)
        {
            return value;
        }

        public override object Value
        {
            get
            {
                if (_dbNullcheckBox.Checked)
                {
                    return DBNull.Value;
                }
                return GetValue(_valueControl.Checked);
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
                    _valueControl.Checked = (bool)value;
                    _dbNullcheckBox.Checked = false;
                }
            }
        }
    }
}
