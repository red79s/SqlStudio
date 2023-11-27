namespace Common
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}
