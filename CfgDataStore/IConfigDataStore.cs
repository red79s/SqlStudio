using Common;
using Common.Model;
using System.Collections.Generic;

namespace CfgDataStore
{
    public interface IConfigDataStore : ICommandHistoryStore
    {

        void AddAlias(string aliasName, string alias);
        void AddAutoQuery(AutoQuery query);
        void AddConnection(Connection con);
        List<Alias> AliasSearch(string alias);
        Connection CreateNewConnection(string providerName);
        string GetAlias(string aliasName);
        List<Alias> GetAliases();
        List<AutoQuery> GetAutoQueries();
        string GetConnectCommand(Connection connection);
        string GetConnectCommand(string provider, string server, string db, string user, string password);
        Connection GetConnection(long key);
        List<Connection> GetConnections();
        Connection GetDefaultConnection();
        long GetLongValue(string name);
        string GetStringValue(string name);
        void Load();
        void ModifyAlias(string aliasName, string alias);
        void Refresh();
        bool RemoveAlias(string aliasName);
        void RemoveAutoQuery(AutoQuery query);
        void RemoveConnection(long key);
        void Save();
        void SetDefaultConnection(long key);
        void SetValue(string name, long value);
        void SetValue(string name, string value);
        void UpdateDatabaseOnConnection(long key, string database);
    }
}