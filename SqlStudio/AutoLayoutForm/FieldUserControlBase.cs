using System;
using System.Drawing;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
#pragma warning disable CA1416
    public class FieldUserControlBase : UserControl, IFieldUserControl
    {
        protected readonly FieldInfo _fieldInfo;
        protected GroupBox _groupBox;
        protected CheckBox _dbNullcheckBox;

        public string FieldName { get; set; }
        public virtual object Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FieldUserControlBase(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            CreateControls();
            ResizeControls();
            Resize += On_Resize;

            _dbNullcheckBox.CheckedChanged += DbNull_CheckedChanged;
        }

        protected virtual void DbNull_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void On_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        protected virtual void CreateControls()
        {
            _groupBox = new GroupBox { Text = $"{_fieldInfo.Name} - {_fieldInfo.ValueType.Name}" };
            Controls.Add(_groupBox);

            _dbNullcheckBox = new CheckBox { Text = "Null", TextAlign = ContentAlignment.MiddleLeft };
            _groupBox.Controls.Add(_dbNullcheckBox);
        }

        protected virtual void ResizeControls()
        {
            _groupBox.Width = Width;
            _groupBox.Height = Height;
            _groupBox.Top = 0;
            _groupBox.Left = 0;

            _dbNullcheckBox.Width = 50;
            _dbNullcheckBox.Height = Height - 11;
            _dbNullcheckBox.Location = new Point(Width - 60, 9);
        }
    }
}
