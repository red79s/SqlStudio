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
        private readonly ILogger _logger;

        public CascadingDeleter(ISqlExecuter sqlExecuter, IDatabaseSchemaInfo databaseSchemaInfo, ILogger logger) 
        {
            _sqlExecuter = sqlExecuter;
            _databaseSchemaInfo = databaseSchemaInfo;
            _logger = logger;
        }

        public List<SqlResult> Delete(string tableName, List<ColumnValue> keys, bool onlyExploreAffectedRows) 
        {
            var res = new List<SqlResult>();
            var sql = GenerateSelect(tableName, keys);
            var tableRes = _sqlExecuter.ExecuteSql(sql);
            
            if (tableRes.DataTable.Rows.Count == 0)
                return res;

            res.Add(tableRes);

            foreach (DataRow row in tableRes.DataTable.Rows )
            {
                var foreignKeysForTables = _databaseSchemaInfo.ForeignKeys.Where(x => x.ForeignTableName == tableName).GroupBy(x => x.TableName);
                foreach (var foreignKeyTable in foreignKeysForTables)
                {
                    var foreignKeys = new List<ColumnValue>();
                    foreach (var item in foreignKeyTable)
                    {
                        foreignKeys.Add(new ColumnValue { Column = item.ColumnName, Value = row[item.ForeignColumnName].ToString() });
                    }

                    res.AddRange(Delete(foreignKeyTable.Key, foreignKeys, onlyExploreAffectedRows));
                }
            }

            return res;
        }

        private string GenerateSelect(string tableName, List<ColumnValue> keys) 
        {
            string sql = $"SELECT * FROM {tableName} WHERE ";
            bool first = true;
            foreach (var key in keys)
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
