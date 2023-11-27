using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Common.Model;

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
            DataTable dtTables = GetTablesTemplate();

            string query = "SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = '" + TableType + "'";
            if (!string.IsNullOrEmpty(tableSearch))
                query += string.Format(" AND table_name LIKE '{0}'", tableSearch);

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

            string query = "SELECT  c.TABLE_NAME, c.COLUMN_NAME,c.DATA_TYPE, c.Column_default, c.character_maximum_length, c.numeric_precision, c.ordinal_position, c.is_nullable" + Environment.NewLine +
                        "             ,CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PRIMARY KEY' ELSE '' END AS KeyType" + Environment.NewLine +
                        "FROM INFORMATION_SCHEMA.COLUMNS c" + Environment.NewLine +
                        "LEFT JOIN (" + Environment.NewLine +
                        "            SELECT ku.TABLE_CATALOG,ku.TABLE_SCHEMA,ku.TABLE_NAME,ku.COLUMN_NAME" + Environment.NewLine +
                        "            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc" + Environment.NewLine +
                        "            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku" + Environment.NewLine +
                        "                ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY' " + Environment.NewLine +
                        "                AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME" + Environment.NewLine +
                        "         )   pk " + Environment.NewLine +
                        "ON  c.TABLE_CATALOG = pk.TABLE_CATALOG" + Environment.NewLine +
                        "            AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA" + Environment.NewLine +
                        "            AND c.TABLE_NAME = pk.TABLE_NAME" + Environment.NewLine +
                        "            AND c.COLUMN_NAME = pk.COLUMN_NAME" + Environment.NewLine +
                        "ORDER BY c.TABLE_SCHEMA,c.TABLE_NAME, c.ORDINAL_POSITION";

            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = Connection;
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
                dr["primary_key"] = "PRIMARY KEY" == (string)reader.GetString("KeyType");
                dtColumns.Rows.Add(dr);
            }
            reader.Close();

            return dtColumns;
        }

        public override List<ForeignKeyInfo> GetForeignKeyInfo()
        {
            var query = "SELECT   " + Environment.NewLine +
                        "    f.name AS foreign_key_name  " + Environment.NewLine +
                        "   ,OBJECT_NAME(f.parent_object_id) AS table_name  " + Environment.NewLine +
                        "   ,COL_NAME(fc.parent_object_id, fc.parent_column_id) AS column_name  " + Environment.NewLine +
                        "   ,OBJECT_NAME (f.referenced_object_id) AS referenced_table " + Environment.NewLine +
                        "   ,COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS referenced_column_name  " + Environment.NewLine +
                        "   ,f.is_disabled, f.is_not_trusted" + Environment.NewLine +
                        "   ,f.delete_referential_action_desc  " + Environment.NewLine +
                        "   ,f.update_referential_action_desc  " + Environment.NewLine +
                        "FROM sys.foreign_keys AS f  " + Environment.NewLine +
                        "INNER JOIN sys.foreign_key_columns AS fc   " + Environment.NewLine +
                        "   ON f.object_id = fc.constraint_object_id   " + Environment.NewLine +
                        "WHERE f.parent_object_id = OBJECT_ID('HumanResources.Employee');";

            DbCommand command = ProviderFactory.CreateCommand();
            command.Connection = Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            var foreignKeys = new List<ForeignKeyInfo>();
            while (reader.Read())
            {
                var foreignKeyInfo = new ForeignKeyInfo();
                foreignKeyInfo.TableName = reader.GetString("table_name");
                foreignKeyInfo.ColumnName = reader.GetString("column_name");
                foreignKeyInfo.ForeignTableName = reader.GetString("referenced_object");
                foreignKeyInfo.ForeignColumnName = reader.GetString("referenced_column_name");
                foreignKeyInfo.ConstraintName = reader.GetString("foreign_key_name");
                foreignKeys.Add(foreignKeyInfo);
            }
            reader.Close();
            return foreignKeys;
        }


        public override List<string> GetDatabases()
        {
            var query = "SELECT name FROM sys.databases ORDER BY name";
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
