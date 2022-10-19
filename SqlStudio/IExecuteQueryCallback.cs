using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio
{
    public interface IExecuteQueryCallback
    {
        void ExecuteQuery(string query, bool inNewTab, string datatabLabel);
    }
}
