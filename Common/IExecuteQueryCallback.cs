using Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public interface IExecuteQueryCallback
    {
        void ExecuteQuery(string query, bool inNewTab, string datatabLabel);
        Task ExecuteCascadingDelete(TableKeyValues tableKeyValues, bool onlyExploreAffectedRows);
    }
}
