using Common.Model;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CfgDataStore
{
    public class ConfigDataStore : IConfigDataStore
    {
        public int MaxHistoryItems { get; set; } = 100;
        ConfigDbContext _dbContext;
        public ConfigDataStore(string cfgFile)
        {
            _dbContext = new ConfigDbContext(cfgFile);
        }

        public void Refresh()
        {
            Save();
        }

        public void Load()
        {
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        #region Values

        public void SetValue(string name, string value)
        {
            var curVal = _dbContext.Cfg_Values.FirstOrDefault(x => x.name == name);
            if (curVal != null)
            {
                curVal.str_value = value;
            }
            else
            {
                _dbContext.Cfg_Values.Add(new CfgValue { p_key = GetKey("cfg_value"), name = name, str_value = value, long_value = 0 });
            }
        }

        public void SetValue(string name, long value)
        {
            var curVal = _dbContext.Cfg_Values.FirstOrDefault(x => x.name == name);
            if (curVal != null)
            {
                curVal.long_value = value;
            }
            else
            {
                _dbContext.Cfg_Values.Add(new CfgValue { p_key = GetKey("cfg_value"), name = name, str_value = "", long_value = value });
            }
        }

        public string GetStringValue(string name)
        {
            var cfgValue = _dbContext.Cfg_Values.FirstOrDefault<CfgValue>(x => x.name == name);
            return cfgValue?.str_value;
        }

        public long GetLongValue(string name)
        {
            var cfgValue = _dbContext.Cfg_Values.FirstOrDefault<CfgValue>(x => x.name == name);
            return cfgValue == null ? 0 : cfgValue.long_value;
        }

        #endregion

        #region Aliases


        public string GetAlias(string aliasName)
        {
            var alias = _dbContext.Aliases.ToList().FirstOrDefault<Alias>(x => x.alias_name.Equals(aliasName, StringComparison.InvariantCultureIgnoreCase));
            return alias?.alias_value;
        }

        public List<Alias> AliasSearch(string alias)
        {
            var aliases = _dbContext.Aliases.Where(x => x.alias_name.Contains(alias));
            return aliases.ToList();
        }

        public List<Alias> GetAliases()
        {
            return _dbContext.Aliases.ToList();
        }

        public void ModifyAlias(string aliasName, string alias)
        {
            var a = _dbContext.Aliases.FirstOrDefault(x => x.alias_name == aliasName);
            if (a != null)
            {
                a.alias_value = alias;
            }
        }

        public void AddAlias(string aliasName, string alias)
        {
            var key = GetKey(nameof(_dbContext.Aliases));
            _dbContext.Aliases.Add(new Alias
            {
                p_key = key,
                alias_name = aliasName,
                alias_value = alias
            });
        }

        public bool RemoveAlias(string aliasName)
        {
            var alias = _dbContext.Aliases.FirstOrDefault(x => x.alias_name == aliasName);
            if (alias != null)
            {
                _dbContext.Aliases.Remove(alias);
                return true;
            }

            return false;
        }
        #endregion

        #region Connections
        public Connection GetConnection(long key)
        {
            return _dbContext.Connections.FirstOrDefault(x => x.p_key == key);
        }

        public string GetConnectCommand(Connection connection)
        {
            if (connection == null)
                return null;

            string ret = string.Format("connect \"{0}\"", connection.provider);

            if (!string.IsNullOrEmpty(connection.server))
                ret += " --s \"" + connection.server + "\"";
            if (!string.IsNullOrEmpty(connection.db))
                ret += " --d \"" + connection.db + "\"";
            if (!string.IsNullOrEmpty(connection.user))
                ret += " --u \"" + connection.user + "\"";
            if (!string.IsNullOrEmpty(connection.password))
                ret += " --p \"" + connection.password + "\"";

            return ret;
        }

        public string GetConnectCommand(string provider, string server, string db, string user, string password)
        {
            string ret = string.Format("connect \"{0}\"", provider);

            if (server != null && server != "")
                ret += " --s \"" + server + "\"";
            if (db != null && db != "")
                ret += " --d \"" + db + "\"";
            if (user != null && user != "")
                ret += " --u \"" + user + "\"";
            if (password != null && password != "")
                ret += " --p \"" + password + "\"";

            return ret;
        }

        public void UpdateDatabaseOnConnection(Connection connection, string database)
        {
            var con = _dbContext.Connections.FirstOrDefault(x => x.p_key == connection.p_key);
            if (con != null)
            {
                con.db = database;
            }
        }

        public void SetDefaultConnection(Connection connection)
        {
            var connections = _dbContext.Connections.ToList();
            foreach (var row in connections)
            {
                if (row.p_key != connection.p_key)
                    row.default_connection = false;
                else
                    row.default_connection = true;
            }
        }

        public Connection GetDefaultConnection()
        {
            return _dbContext.Connections.FirstOrDefault(x => x.default_connection == true);
        }

        public List<Connection> GetConnections()
        {
            return _dbContext.Connections.ToList();
        }

        public Connection CreateNewConnection(string providerName)
        {
            var con = new Connection
            {
                provider = providerName,
                description = "New Connection",
                server = "",
                db = "",
                user = "",
                password = "",
                integrated_security = false,
                default_connection = false
            };

            return con;
        }

        public void AddConnection(Connection con)
        {
            con.p_key = GetKey(nameof(_dbContext.Connections));
            _dbContext.Connections.Add(con);
        }

        public void RemoveConnection(Connection connection)
        {
            var con = _dbContext.Connections.FirstOrDefault(x => x.p_key == connection.p_key);
            if (con != null)
            {
                _dbContext.Connections.Remove(con);
            }
        }
        #endregion

        #region History
        public List<CommandHistoryItem> GetHistoryItems()
        {
            List<CommandHistoryItem> ret = new List<CommandHistoryItem>();
            foreach (var item in _dbContext.HistoryLog.ToList())
            {
                ret.Add(new CommandHistoryItem { Id = item.Id, Command = item.Command, LastExecuted = item.LastExecuted });
            }
            return ret;
        }

        public void SetHistoryItems(List<string> items)
        {
            var existingItems = _dbContext.HistoryLog.ToList();

            foreach (var item in items)
            {
                var existing = existingItems.FirstOrDefault(x => x.Command == item);
                if (existing != null)
                {
                    _dbContext.HistoryLog.Remove(existing);
                }
                _dbContext.HistoryLog.Add(new HistoryLogItem
                {
                    Command = item,
                    LastExecuted = DateTime.Now
                });
            }
        }

        public void ClearHistory()
        {
            _dbContext.HistoryLog.RemoveRange(_dbContext.HistoryLog);
        }

        public void AddHistoryItem(string command)
        {
            var items = _dbContext.HistoryLog.ToList();
            int removedItems = 0;
            foreach (var item in items)
            {
                if (item.Command == command)
                {
                    _dbContext.HistoryLog.Remove(item);
                    removedItems++;
                }
            }

            if (items.Count > MaxHistoryItems)
            {
                var numItemsToRemove = items.Count - MaxHistoryItems - removedItems;
                if (numItemsToRemove > 0)
                {
                    for (int i = 0; i < numItemsToRemove; i++)
                    {
                        _dbContext.HistoryLog.Remove(items[i]);
                    }
                }
            }

            _dbContext.HistoryLog.Add(new HistoryLogItem { Command = command, LastExecuted = DateTime.Now });

            _dbContext.SaveChanges();
        }

        #endregion
        #region AutoQueries
        public List<AutoQuery> GetAutoQueries()
        {
            return _dbContext.AutoQueries.ToList();
        }

        public void AddAutoQuery(AutoQuery query)
        {
            _dbContext.AutoQueries.Add(query);
        }

        public void RemoveAutoQuery(AutoQuery query)
        {
            _dbContext.AutoQueries.Remove(query);
        }
        #endregion

        private long GetKey(string tableName)
        {
            var key = _dbContext.Keys.FirstOrDefault(x => x.table_name == tableName.ToLower());
            key.current_key++;
            return key.current_key;
        }
    }
}
