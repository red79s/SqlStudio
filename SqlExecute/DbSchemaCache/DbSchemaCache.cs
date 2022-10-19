using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace SqlExecute.DbSchemaCache
{
    public class DbSchemaCache
    {
        private SQLiteConnection _connection = null;
        private ColumnCacheDataTable _dtColCache = null;
        public DbSchemaCache()
        {
            this._connection = new SQLiteConnection("Data Source = :memory:");
            this._connection.Open();

            this._dtColCache = new ColumnCacheDataTable(SQLiteFactory.Instance, this._connection);
            this._dtColCache.CreateTable();
        }

        public DbSchemaCache(SQLiteConnection connection)
        {
            this._connection = connection;
            this._dtColCache = new ColumnCacheDataTable(SQLiteFactory.Instance, this._connection);
        }

        public ColumnCacheDataTable ColumnCache
        {
            get { return this._dtColCache; }
        }

        public List<string> GetTables(string tableSearch)
        {
            List<string> ret = new List<string>();

            string sql = string.Format("select distinct {0} from {1} where {0} like '{2}%'",
                ColumnCacheDataRow.table_nameColumn, this._dtColCache.TableName, tableSearch);
            SQLiteCommand com = new SQLiteCommand(sql, this._connection);
            SQLiteDataReader read = com.ExecuteReader();
            while (read.Read())
            {
                ret.Add(read.GetString(0));
            }
            read.Close();

            return ret;
        }

        public void Clear()
        {
            this._dtColCache.TruncateTable();
        }
    }
}
