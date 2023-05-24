using System;
using System.Windows.Forms;

namespace SqlStudio.AutoLayoutForm
{
    public partial class UserControlDateTime : UserControl, IFieldUserControl
    {
        public UserControlDateTime(FieldInfo fieldInfo)
        {
            InitializeComponent();
            FieldName = fieldInfo.Name;
            Value = fieldInfo.Value;
        }

        public object Value 
        { 
            get => dtpValue.Value; 
            set
            {
                dtpValue.Value = (DateTime)value;
            } 
        }
        public string FieldName
        {
            get => lblName.Text;
            set => lblName.Text = value;
        }
    }
}
