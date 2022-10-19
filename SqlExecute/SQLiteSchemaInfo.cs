using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace SqlExecute
{
    class SQLiteSchemaInfo : DBSchemaInfoBase
    {
        public SQLiteSchemaInfo(DbConnection connection, DbProviderFactory factory)
            : base(connection, factory)
        {
        }

        public override System.Data.DataTable GetTableInfo(string tableSearch)
        {
            DataTable dtTables = this.GetTablesTemplate();

            string query = "SELECT tbl_name FROM sqlite_master WHERE type = 'table'";
            if (tableSearch != null && tableSearch != "")
                query += string.Format(" AND tbl_name LIKE '{0}'", tableSearch);

            DbCommand command = this.ProviderFactory.CreateCommand();
            command.Connection = this.Connection;
            command.CommandText = query;
            DbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DataRow dr = dtTables.NewRow();
                dr["table_name"] = reader["tbl_name"];
                dtTables.Rows.Add(dr);
            }
            reader.Close();

            return dtTables;
        }

        public override DataTable GetColumnsInfo(string tableSearch, string columnSearch)
        {
            string columnRestriction = columnSearch.Replace("%", "");
            DataTable dtColumns = this.GetColumnsTemplate();

            DataTable dtTables = this.GetTableInfo(tableSearch);

            foreach (DataRow drTable in dtTables.Rows)
            {
                string query = "SELECT * FROM " + drTable["table_name"];

                DbCommand command = this.ProviderFactory.CreateCommand();
                command.Connection = this.Connection;
                command.CommandText = query;
                DbDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
                DataTable schema = reader.GetSchemaTable();
                foreach (DataRow drSchema in schema.Rows)
                {
                    
                    string columnName = (string)drSchema[SchemaTableColumn.ColumnName];
                    if (columnRestriction == null || columnRestriction == "" || columnName.IndexOf(columnRestriction) >= 0)
                    {
                        DataRow dr = dtColumns.NewRow();
                        dr["table_name"] = drTable["table_name"];
                        dr["column_name"] = columnName;
                        dr["ordinal_position"] = drSchema[SchemaTableColumn.ColumnOrdinal];
                        dr["data_type"] = drSchema[24];
                        dr["column_length"] = drSchema[SchemaTableColumn.ColumnSize];
                        dr["column_precision"] = drSchema[SchemaTableColumn.NumericPrecision];
                        dr["is_nullable"] = drSchema[SchemaTableColumn.AllowDBNull];
                        dr["primary_key"] = drSchema[SchemaTableColumn.IsKey];
                        dtColumns.Rows.Add(dr);
                    }
                }
                reader.Close();
            }

            return dtColumns;
        }
    }
}
