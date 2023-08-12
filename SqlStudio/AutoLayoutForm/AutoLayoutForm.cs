using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SqlStudio.AutoLayoutForm
{
    public partial class AutoLayoutForm : Form
    {
        private UserControl _contentControl;
        private SortedDictionary<FieldInfo, IFieldUserControl> _fieldControls = new SortedDictionary<FieldInfo, IFieldUserControl>();
        public AutoLayoutForm(SortedDictionary<int, List<FieldInfo>> fieldInfos)
        {
            InitializeComponent();
            CreateControls(fieldInfos);
            ResizeControls();
            Resize += AutoLayoutForm_Resize;
        }

        private void AutoLayoutForm_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void CreateControls(SortedDictionary<int, List<FieldInfo>> fieldInfos)
        {
            _contentControl = new UserControl();
            Controls.Add(_contentControl);

            foreach (var fieldInfo in fieldInfos)
            {
                var control = new RowFieldsControl(fieldInfo.Key, fieldInfo.Value);
                _contentControl.Controls.Add(control);
            }
        }

        private void ResizeControls()
        {
            _contentControl.Width = Width;
            _contentControl.Height = Height - 30;
            _contentControl.Location = new Point(0, 0);

            var row = 0;
            foreach (var control in _contentControl.Controls)
            {
                var c = control as RowFieldsControl;
                c.Location = new Point(3, (row * c.TotalHeight));
                c.Width = _contentControl.Width -23;
                c.Height = c.TotalHeight;
                row++;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SaveButton_Click(object sender, System.EventArgs e)
        {
            foreach (RowFieldsControl control in _contentControl.Controls)
            {
                control.SetValuesFromControls();
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
