namespace Common.Model
{
    public class ForeignKeyInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ForeignTableName { get; set; }
        public string ForeignColumnName { get; set; }
        public string ConstraintName { get; set; }
        public bool IsCascadeDelete { get; set; }
    }
}
