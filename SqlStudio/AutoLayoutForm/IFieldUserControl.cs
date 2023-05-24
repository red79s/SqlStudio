using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.AutoLayoutForm
{
    public interface IFieldUserControl
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
