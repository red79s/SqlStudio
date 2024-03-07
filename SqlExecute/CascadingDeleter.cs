using Common;
using Common.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlExecute
{
    public class CascadingDeleter
    {
        private readonly ISqlExecuter _sqlExecuter;
        private readonly IDatabaseSchemaInfo _databaseSchemaInfo;
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;
        private readonly ILogger _logger;

        public CascadingDeleter(ISqlExecuter sqlExecuter, IDatabaseSchemaInfo databaseSchemaInfo, IDatabaseKeywordEscape databaseKeywordEscape, ILogger logger) 
        {
            _sqlExecuter = sqlExecuter;
            _databaseSchemaInfo = databaseSchemaInfo;
            _databaseKeywordEscape = databaseKeywordEscape;
            _logger = logger;
        }

        public List<SqlResult> Delete(TableKeyValues tableKeyValues, bool onlyExploreAffectedRows, List<TableKeyValues> parentTableKeys = null) 
        {
            var res = new List<SqlResult>();

			if (parentTableKeys == null)
				parentTableKeys = new List<TableKeyValues>();

            if (HaveVisited(tableKeyValues, parentTableKeys))
            {
                _logger.Log(LogLevel.Warn, $"CascadingDeleter::Delete tableName: {tableKeyValues.TableName}, {string.Join(", ", tableKeyValues.Keys.Select(x => x.Value))}, already explored");
				return res;
			}
            
            parentTableKeys.Add(tableKeyValues);

			var sql = GenerateSelect(tableKeyValues);
            var tableRes = _sqlExecuter.ExecuteSql(sql);
            tableRes.DisplayAsText = true;
            
            if (tableRes.DataTable.Rows.Count == 0)
                return res;

            _logger.Log(LogLevel.Debug, $"CascadingDeleter::FindRows tableName: {tableKeyValues.TableName}, {string.Join(", ", tableKeyValues.Keys.Select(x => x.Value))}, rows: {tableRes.DataTable.Rows.Count}");

            res.Add(tableRes);

            foreach (DataRow row in tableRes.DataTable.Rows )
            {
                var foreignKeysForTables = _databaseSchemaInfo.ForeignKeys.Where(x => x.ForeignTableName == tableKeyValues.TableName).GroupBy(x => x.TableName);
                foreach (var foreignKeyTable in foreignKeysForTables)
                {
                    var foreignKeys = new List<ColumnValue>();
                    foreach (var item in foreignKeyTable)
                    {
                        foreignKeys.Add(new ColumnValue { Column = item.ColumnName, Value = row[item.ForeignColumnName].ToString() });
                    }

                    res.AddRange(Delete(new TableKeyValues { TableName = foreignKeyTable.Key, Keys = foreignKeys }, onlyExploreAffectedRows, parentTableKeys));
                }
            }

            if (!onlyExploreAffectedRows && tableRes.DataTable.Rows.Count > 0)
            {
                var delSql = GenerateDelete(tableKeyValues);
                var delRes = _sqlExecuter.ExecuteSql(delSql);
                if (delRes.Success)
                {
                    _logger.Log(LogLevel.Debug, $"CascadingDeleter::Delete tableName: {tableKeyValues.TableName}, {string.Join(", ", tableKeyValues.Keys.Select(x => x.Value))}, rows: {delRes.RowsAffected}");
                }
                else
                {
                    _logger.Log(LogLevel.Debug, $"CascadingDelete::Delete error: {delRes.Message}, tableName: {tableKeyValues.TableName}, {string.Join(", ", tableKeyValues.Keys.Select(x => x.Value))}");
                }
            }

            return res;
        }

        private bool HaveVisited(TableKeyValues tableKeyValues, List<TableKeyValues> parentTables)
        {
            foreach (var parentTable in parentTables)
            {
                if (parentTable.TableName == tableKeyValues.TableName)
                {
                    var haveVisited = true;
                    foreach (var key in  parentTable.Keys)
                    {
                        var col = tableKeyValues.Keys.FirstOrDefault(x => x.Column == key.Column);
                        if (col == null || col.Value != key.Value)
                        {
                            haveVisited = false;
                        }
                    }

                    if (haveVisited)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string GenerateSelect(TableKeyValues tableKeyValues) 
        {
            string sql = $"SELECT * FROM {_databaseKeywordEscape.EscapeObject(tableKeyValues.TableName)} WHERE ";
            bool first = true;
            foreach (var key in tableKeyValues.Keys)
            {
                if (!first)
                    sql += " AND ";
                first = false;

                sql += $"{key.Column} = {key.Value}";
            }
            return sql;
        }

        private string GenerateDelete(TableKeyValues tableKeyValues)
        {
            string sql = $"DELETE FROM {_databaseKeywordEscape.EscapeObject(tableKeyValues.TableName)} WHERE ";
            bool first = true;
            foreach (var key in tableKeyValues.Keys)
            {
                if (!first)
                    sql += " AND ";
                first = false;

                sql += $"{key.Column} = {key.Value}";
            }
            return sql;
        }
    }
}
