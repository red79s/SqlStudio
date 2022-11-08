
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace SqlExecute.DbSchemaCache
{
    public class DbSchemaCache
    {
        private SQLiteConnection _connection = null;
        private ColumnCacheDataTable _dtColCache = null;
        public DbSchemaCache()
        {
            _connection = new SQLiteConnection("Data Source = :memory:");
            _connection.Open();

            _dtColCache = new ColumnCacheDataTable(SQLiteFactory.Instance, _connection);
            _dtColCache.CreateTable();
        }

        public DbSchemaCache(SQLiteConnection connection)
        {
            _connection = connection;
            _dtColCache = new ColumnCacheDataTable(SQLiteFactory.Instance, _connection);
        }

        public ColumnCacheDataTable ColumnCache
        {
            get { return _dtColCache; }
        }

        public List<string> GetTables(string tableSearch)
        {
            List<string> ret = new List<string>();

            string sql = string.Format("select distinct {0} from {1} where {0} like '%{2}%'",
                ColumnCacheDataRow.table_nameColumn, _dtColCache.TableName, tableSearch);
            var com = new SQLiteCommand(sql, _connection);
            var read = com.ExecuteReader();
            while (read.Read())
            {
                ret.Add(read.GetString(0));
            }
            read.Close();

            return ret;
        }

        public void Clear()
        {
            _dtColCache.TruncateTable();
        }
    }
}
