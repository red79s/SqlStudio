using Common.Model;
using System.Collections.Generic;

namespace Common
{
    public interface IExecuteQueryCallback
    {
        void ExecuteQuery(string query, bool inNewTab, string datatabLabel);
        void ExecuteCascadingDelete(string tablename, List<ColumnValue> keys, bool onlyExploreAffectedRows);
    }
}
