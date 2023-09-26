using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public class RowFieldsControl : UserControl
    {
        private GroupBox _groupBox;
        private int MinFieldWidth = 200;
        private int MaxFieldWidth = 200;
        private readonly List<FieldInfo> _fields;

        public int TotalHeight { get; set; }

        public RowFieldsControl(int rowIndex, List<FieldInfo> fields)
        {
            _fields = fields;

            CreateControls(rowIndex, fields);
            ResizeControls();
            Resize += RowFieldsControl_Resize;
        }

        private void RowFieldsControl_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void CreateControls(int  rowIndex, List<FieldInfo> fields)
        {
            _groupBox = new GroupBox { Text = $"Row: {rowIndex}" };
            Controls.Add(_groupBox);

            foreach (FieldInfo field in fields)
            {
                _groupBox.Controls.Add(CreateFieldUserControl(field));
            }
        }

        private void ResizeControls()
        {
            _groupBox.Location = new System.Drawing.Point(0, 0);
            _groupBox.Width = Width;
            _groupBox.Height = Height;
            int contentWidht = _groupBox.Width - 10;
            int columns = contentWidht / MinFieldWidth;
            if (columns > _fields.Count)
            {
                columns = _fields.Count;
            }
            if (columns < 1)
            {
                columns = 1;
            }
            int rows = 0;
            int columnWidht = (contentWidht / columns) - 10;
            for (int i = 0; i < _groupBox.Controls.Count; i++)
            {
                int column = i % columns;
                int row = i / columns;
                rows = row + 1;
                _groupBox.Controls[i].Width = columnWidht;
                _groupBox.Controls[i].Left = 5 + (column * (columnWidht + 10));
                
                _groupBox.Controls[i].Top = 15 + (row * 48);
                _groupBox.Controls[i].Height = 45;
            }

            TotalHeight = (rows * 48) + 15;
        }

        public void SetValuesFromControls()
        {
            for (int i = 0; i < _fields.Count; i++)
            {
                var control = _groupBox.Controls[i] as IFieldUserControl;
                _fields[i].Value = control.Value;
            }
        }

        private UserControl CreateFieldUserControl(FieldInfo fieldInfo)
        {
            if (fieldInfo.ValueType == typeof(DateTime))
            {
                return new FieldUserControlDateTime(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(int))
            {
                return new FieldUserControlInt(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(long))
            {
                return new FieldUserControlLong(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(decimal))
            {
                return new FieldUserControlDecimal(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(float))
            {
                return new FieldUserControlFloat(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(double))
            {
                return new FieldUserControlDouble(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(bool))
            {
                return new FieldUserControlBool(fieldInfo);
            }

            if (fieldInfo.ValueType == typeof(string))
            {
                return new FieldUserControlText(fieldInfo);
            }

            return new FieldUserControlBase(fieldInfo);
        }
    }
}
