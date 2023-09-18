using Common.Model;
using System.Collections.Generic;

namespace Common
{
    public interface IColumnValueDescriptionProvider
    {
        string GetDescriptionForValue(string tableName, string columnName, string value);
        List<string> GetDescriptionForColumn(string tableName, string columnName);
        void AddColumnMetadataInfo(string source, List<ColumnValueDescription> columnValues);
        void Load();
        void Save();
    }
}
