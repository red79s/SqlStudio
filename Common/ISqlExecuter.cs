using Common.Model;
using System.Collections.Generic;
using System.Data.Common;

namespace Common
{
    public interface ISqlExecuter
    {
        DbConnection Connection { get; }
        string ConnectionString { get; }
        string CurrentScriptPath { get; set; }
        string DatabaseName { get; }
        IList<ForeignKeyInfo> ForeignKeys { get; }
        IList<TableInfo> Tables { get; }

        void Cancel();
        List<SqlResult> DeleteCascading(string tableName, List<ColumnValue> keyValues, bool onlyDisplayAffectedRows);
        void Disconnect();
        SqlResult ExecuteSql(string sql, DbTransaction transaction = null);
        List<SqlResult> ExecuteSql(string[] sql);
        void GetColumns(string tableSearch, string columnSearch);
        List<string> GetDatabases();
        void GetMetaData();
        void GetSchema(string schema, string[] restrictions);
        void GetTables(string tableSearch);
        bool HaveConnection();
        void SaveResults(SqlResult result);
        void SetTimeout(int iTimeout);
    }
}