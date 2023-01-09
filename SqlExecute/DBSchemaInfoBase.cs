using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SqlExecute
{
    class DBSchemaInfoBase
    {
        private DbConnection _connection = null;
        private DbProviderFactory _factory = null;

        public DBSchemaInfoBase(DbConnection connection, DbProviderFactory factory)
        {
            _connection = connection;
            _factory = factory;
        }

        public static DBSchemaInfoBase GetSchemaClass(SqlExecute.SqlExecuter.DatabaseProvider provider, DbConnection connection, DbProviderFactory factory)
        {
            switch (provider)
            {
                case SqlExecuter.DatabaseProvider.SQLITE: return (DBSchemaInfoBase)new SQLiteSchemaInfo(connection, factory);
                case SqlExecuter.DatabaseProvider.ORACLE: return (DBSchemaInfoBase)new OracleSchemaInfo(connection, factory);
                case SqlExecuter.DatabaseProvider.SQLSERVER: return (DBSchemaInfoBase)new SqlServerSchemaInfo(connection, factory);
                case SqlExecuter.DatabaseProvider.POSTGRESQL: return (DBSchemaInfoBase)new PostgreSqlSchemaInfo(connection, factory);
                case SqlExecuter.DatabaseProvider.SQLSERVERCE: return (DBSchemaInfoBase)new SqlServerSchemaInfo(connection, factory);
            }
            return (DBSchemaInfoBase)new DBSchemaInfoBase(connection, factory);
        }

        protected DbConnection Connection
        {
            get { return _connection; }
        }

        protected DbProviderFactory ProviderFactory
        {
            get { return _factory; }
        }

        public virtual DataTable GetTableInfo(string tableSearch)
        {
            DataTable dtTables = GetTablesTemplate();
            DataTable dtTablesSchema = _connection.GetSchema("TABLES");
            foreach (DataRow drSchema in dtTablesSchema.Rows)
            {
                DataRow dr = dtTables.NewRow();
                dr["table_name"] = drSchema["table_name"];
                dtTables.Rows.Add(dr);
            }

            return dtTables;
        }

        public virtual List<string> GetDatabases()
        {
            return new List<string> { _connection.Database };
        }

        public virtual DataTable GetColumnsInfo(string tableSearch, string columnSearch)
        {
            DataTable dtColumns = GetColumnsTemplate();
            DataTable dtColumnsSchema = _connection.GetSchema("COLUMNS");
            foreach (DataRow drSchema in dtColumnsSchema.Rows)
            {
                DataRow dr = dtColumns.NewRow();
                dr["table_name"] = drSchema["table_name"];
                dr["column_name"] = drSchema["column_name"];
                dr["ordinal_position"] = drSchema["ordinal_position"];
                dr["data_type"] = drSchema["data_type"];
                //dr["is_nullable"] = bool.Parse(drSchema["is_nullable"]);
                //dr["primary_key"] = bool.Parse(drSchema["primary_key"]);
                dtColumns.Rows.Add(dr);
            }

            return dtColumns;
        }

        public virtual DataTable GetSchema()
        {
            return _connection.GetSchema();
        }

        public virtual DataTable GetSchema(string collectionName)
        {
            return _connection.GetSchema(collectionName);
        }

        public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _connection.GetSchema(collectionName, restrictionValues);
        }

        protected DataTable GetTablesTemplate()
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("table_name", typeof(string));

            return dt;
        }

        protected DataTable GetColumnsTemplate()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("table_name", typeof(string));
            dt.Columns.Add("column_name", typeof(string));
            dt.Columns.Add("ordinal_position", typeof(int));
            dt.Columns.Add("data_type", typeof(string));
            dt.Columns.Add("column_length", typeof(int));
            dt.Columns.Add("column_precision", typeof(int));
            dt.Columns.Add("is_nullable", typeof(bool));
            dt.Columns.Add("primary_key", typeof(bool));

            return dt;
        }
    }
}
