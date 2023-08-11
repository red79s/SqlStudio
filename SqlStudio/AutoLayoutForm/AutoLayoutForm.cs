using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class AutoLayoutForm : Form
    {
        private Dictionary<FieldInfo, IFieldUserControl> _fieldControls = new Dictionary<FieldInfo, IFieldUserControl>();
        public AutoLayoutForm(List<FieldInfo> fieldInfos)
        {
            InitializeComponent();
            InitFields(fieldInfos, 2);
        }

        private void InitFields(List<FieldInfo> fieldInfos, int columns)
        {
            var columnWith = (Width - 10) / columns;
            var rowHeight = 50;

            var currentY = 5 - rowHeight;

            for (int i = 0; i < fieldInfos.Count; i++)
            {
                var col = i % columns;
                if (col == 0)
                {
                    currentY += rowHeight;
                }

                var fieldInfo = fieldInfos[i];
                var control = CreateFieldUserControl(fieldInfo);
                _fieldControls.Add(fieldInfo, (IFieldUserControl)control);
                control.Left = (columnWith * col) + 5;
                control.Width = columnWith - 10;
                control.Top = currentY;
                control.Height = rowHeight - 5;
                Controls.Add(control);
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

            if (fieldInfo.ValueType == typeof(float))
            {
                return new FieldUserControlDecimal(fieldInfo);
            }

            return new FieldUserControlBase(fieldInfo);

            //return new FieldUserControlDefault(fieldInfo);
        }
        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            foreach (var field in _fieldControls)
            {
                field.Key.Value = field.Value.Value;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
