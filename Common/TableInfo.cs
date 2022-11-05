using System.Collections.Generic;

namespace Common
{
    public class TableInfo
    {
        public string TableName { get; set; }
        public IList<ColumnInfo> Columns { get; set; }
    }
}
