﻿using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class FieldUserControlInt : FieldUserControlBase
    {
        private TextBox _valueControl;
        public FieldUserControlInt(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            Value = fieldInfo.Value;
        }

        protected override void CreateControls()
        {
            base.CreateControls();

            _valueControl = new TextBox();
            _groupBox.Controls.Add(_valueControl);

            _valueControl.Validating += _valueControl_Validating;
        }

        private Regex _validateRegex = new Regex("[0-9 ]+");
        private void _valueControl_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_validateRegex.IsMatch(_valueControl.Text))
                e.Cancel = true;
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
        
        private int GetValue(string text)
        {
            if (text == null) 
                return 0;

            if (int.TryParse(text, out var value))
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