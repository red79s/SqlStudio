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

        public FieldUserControlBool(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            Value = fieldInfo.Value;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            _valueControl = new CheckBox();// { Appearance = Appearance.Button };
            _groupBox.Controls.Add(_valueControl);
        }

        protected override void ResizeControls()
        {
            base.ResizeControls();

            _valueControl.Height = Height - 15;
            _valueControl.Width = Width - 75;
            _valueControl.Location = new Point(10, 13);
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
