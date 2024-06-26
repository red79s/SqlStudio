using Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public interface IExecuteQueryCallback
    {
        void ExecuteQueryAndDisplay(string query, bool inNewTab, string datatabLabel);
        SqlResult ExecuteQuery(string query);
        Task ExecuteCascadingDelete(TableKeyValues tableKeyValues, bool onlyExploreAffectedRows);
    }
}
