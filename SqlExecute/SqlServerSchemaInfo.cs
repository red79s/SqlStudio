using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SqlExecute
{
    class SqlServerSchemaInfo : DBSchemaInfoBase
    {
        private string TableType { get; set; }

        public SqlServerSchemaInfo(DbConnection connection, DbProviderFactory factory)
            : base(connection, factory)
        {
            TableType = /*factory is SqlCeProviderFactory ? "TABLE" :*/ "BASE TABLE";
        }

        public override DataTable GetTableInfo(string tableSearch)
        {
            DataTable dtTables = this.GetTablesTemplate();

            string query = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = '" + TableType + "'";
            if (!string.IsNullOrEmpty(tableSearch))
                query += string.Format(" AND table_name LIKE '{0}'", tableSearch);

            DbCommand command = this.ProviderFactory.CreateCommand();
            command.Connection = this.Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DataRow dr = dtTables.NewRow();
                dr["table_name"] = reader["table_name"];
                dtTables.Rows.Add(dr);
            }
            reader.Close();

            return dtTables;
        }

        public override DataTable GetColumnsInfo(string tableSearch, string columnSearch)
        {
            DataTable dtColumns = this.GetColumnsTemplate();
            
            string query = "SELECT table_name, column_name, ordinal_position, data_type, character_maximum_length, numeric_precision, is_nullable FROM INFORMATION_SCHEMA.COLUMNS WHERE (0 = 0)";
            if (tableSearch != null && tableSearch != "")
                query += string.Format(" AND table_name LIKE '{0}'", tableSearch);
            if (columnSearch != null && columnSearch != "")
                query += string.Format(" AND column_name LIKE '{0}'", columnSearch);

            DbCommand command = this.ProviderFactory.CreateCommand();
            command.Connection = this.Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DataRow dr = dtColumns.NewRow();
                dr["table_name"] = reader["table_name"];
                dr["column_name"] = reader["column_name"];
                dr["ordinal_position"] = reader["ordinal_position"];
                dr["data_type"] = reader["data_type"];
                dr["column_length"] = reader["character_maximum_length"];
                dr["column_precision"] = reader["numeric_precision"];
                dr["is_nullable"] = "NO" != (string)reader["is_nullable"];
                dr["primary_key"] = false;
                dtColumns.Rows.Add(dr);
            }
            reader.Close();

            return dtColumns;
        }

        public override List<string> GetDatabases()
        {
            var query = "SELECT name from sys.databases";
            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            var databases = new List<string>();
            while (reader.Read())
            {
                databases.Add(reader.GetString(0));
            }
            reader.Close();
            return databases;
        }
    }
}
