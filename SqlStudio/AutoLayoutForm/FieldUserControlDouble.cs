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
    internal class FieldUserControlDouble : FieldUserControlBase
    {
        private TextBox _valueControl;
        private ErrorProvider _errorProvider;

        public FieldUserControlDouble(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            Value = fieldInfo.Value;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            _valueControl = new TextBox { TextAlign = HorizontalAlignment.Right };
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
            if (!_validateRegex.IsMatch(_valueControl.Text))
            {
                e.Cancel = true;
                _errorProvider.SetError(_valueControl, "Not a valid int");
            }
            else
            {
                _errorProvider.SetError(_valueControl, "");
            }
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
                _valueControl.Text = "0";
                _valueControl.Enabled = true;
            }
            else
            {
                _valueControl.Text = "";
                _valueControl.Enabled = false;
            }
        }

        private double GetValue(string text)
        {
            if (text == null)
                return 0;

            if (double.TryParse(text, out var value))
                return value;

            return 0;
        }

        public override object Value
        {
            get
            {
                if (_dbNullcheckBox.Checked)
                {
                    return DBNull.Value;
                }
                return GetValue(_valueControl.Text);
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
                    _valueControl.Text = value.ToString();
                    _dbNullcheckBox.Checked = false;
                }
            }
        }
    }
}
