using CfgDataStore;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace SqlStudio.Interfaces
{
    public interface IDatabaseConnectionUserControl
    {
        void Connect(Connection connectionInfo);
        void CloseConnection();
        void CancelExecution();
        void ExecuteQueryAndDisplay(string query, bool inNewTab, string datatabLabel);
        void SetDislayFilterRow(bool showFilterRow);
        void InsertNewDataTab();
        void CreateNewScriptTab();
        void CloseScriptTab();
        void OpenScriptFile();
        void SaveScript();
        void SaveScriptAs();
        void RunScript();
        void Cut();
        void Copy();
        void Paste();
        void OpenSqlite();
        void OpenSqlCet();
        void OpenConfigDb();
        void OpenCvsFile();
        void CopyConnectionStringToClipboard();
        void OpenGenerateDataTool();
        void OpenSetUserPermissionsTool();
        void WriteToOutput(string message);
    }
}
