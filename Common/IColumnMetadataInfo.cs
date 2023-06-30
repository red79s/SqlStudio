namespace Common
{
    public interface IColumnMetadataInfo
    {
        string GetDescriptionForValue(string tableName, string columnName, int value);
    }
}
