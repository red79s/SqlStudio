using System.Collections.Generic;

namespace SqlStudio.EnumImporter
{
    public class ColumnDescription
    {
        public string AssemblyName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public List<EnumValue> EnumValues { get; set; } = new List<EnumValue>();
    }
}
