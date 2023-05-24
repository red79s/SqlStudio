using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class AutoLayoutForm : Form
    {
        public AutoLayoutForm(List<FieldInfo> fieldInfos)
        {
            InitializeComponent();
            InitFields(fieldInfos, 2);
        }

        private void InitFields(List<FieldInfo> fieldInfos, int columns)
        {
            var columnWith = (Width - 10) / columns;
            var rowHeight = 30;

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
                return new UserControlDateTime(fieldInfo);
            }

            return new FieldUserControlDefault(fieldInfo);
        }
        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
