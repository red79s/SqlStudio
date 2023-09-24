using CfgDataStore;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace SqlStudio.Interfaces
{
    public interface IDatabaseConnectionUserControl
    {
        void Connect(Connection connectionInfo);
        void SetDislayFilterRow(bool showFilterRow);
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
    }
}
