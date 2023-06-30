using Common;

namespace SqlStudio.ColumnMetaDataInfo
{
    public class ColumnMetaDataInfoManager : IColumnMetadataInfo
    {
        public string GetDescriptionForValue(string tableName, string columnName, int value)
        {

            if (tableName == "Article" && columnName == "ExpirationType" && value == 0)
                return "None";

            return null;
        }
    }
}
