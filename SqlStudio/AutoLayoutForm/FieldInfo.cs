using System;

namespace SqlStudio.AutoLayoutForm
{
    public class FieldInfo
    {
        public int ColumnIndex { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Type ValueType { get; set; }
        public object Value { get; set; }
    }
}
