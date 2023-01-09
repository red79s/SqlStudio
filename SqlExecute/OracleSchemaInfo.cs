using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SqlExecute
{
    //look in dictionary and dict_columns for future expansions
    class OracleSchemaInfo : DBSchemaInfoBase
    {
        public OracleSchemaInfo(DbConnection connection, DbProviderFactory factory)
            : base(connection, factory)
        {
        }

        public override DataTable GetTableInfo(string tableSearch)
        {
            DataTable dtTables = GetTablesTemplate();

            string query = "SELECT table_name FROM user_tables";
            if (tableSearch != null && tableSearch != "")
                query += string.Format(" WHERE table_name LIKE '{0}'", tableSearch.ToUpper());

            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = Connection;
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
            DataTable dtColumns = GetColumnsTemplate();

            string query = "SELECT table_name, column_name, data_type, nullable, column_id FROM user_tab_cols WHERE (0 = 0)";
            if (tableSearch != null && tableSearch != "")
                query += string.Format(" AND table_name LIKE '{0}'", tableSearch.ToUpper());
            if (columnSearch != null && columnSearch != "")
                query += string.Format(" AND column_name LIKE '{0}'", columnSearch.ToUpper());

            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DataRow dr = dtColumns.NewRow();
                dr["table_name"] = reader["table_name"];
                dr["column_name"] = reader["column_name"];
                dr["ordinal_position"] = reader["column_id"];
                dr["data_type"] = reader["data_type"];
                dr["is_nullable"] = "N" != (string)reader["nullable"];
                dr["primary_key"] = false;
                dtColumns.Rows.Add(dr);
            }
            reader.Close();

            return dtColumns;
        }
    }
}
