using System.Collections.Generic;

namespace Common.Model
{
    public class TableKeyValues
    {
        public string TableName { get; set; }
        public List<ColumnValue> Keys { get; set; }
    }
}
