using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class FieldUserControlDefault : UserControl, IFieldUserControl
    {
        public FieldUserControlDefault(FieldInfo fieldInfo)
        {
            InitializeComponent();
            FieldLabel.Text = fieldInfo.Name;
            tbValue.Text = fieldInfo.Value.ToString();
        }

        public string FieldName { get => FieldLabel.Text; set => FieldLabel.Text = value; }
        public object Value { get => tbValue.Text; set => tbValue.Text = value.ToString(); }
    }
}
