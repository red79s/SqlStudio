using Common.Model;
using System.Collections.Generic;

namespace Common
{
    public interface IColumnValueDescriptionProvider
    {
        string GetDescriptionForValue(string tableName, string columnName, string value);
        void AddColumnMetadataInfo(string source, List<ColumnValueDescription> columnValues);
        void Load();
        void Save();
    }
}
