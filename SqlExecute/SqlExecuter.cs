
using Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace SqlExecute
{
    public class SqlExecuter : IDatabaseSchemaInfo
    {
        public string ConnectionString { get; private set; }
        public enum DatabaseProvider 
        { 
            SQLSERVER, 
            SQLITE, 
            ORACLE, 
            ODBC, 
            POSTGRESQL, 
            SQLSERVERCE,
            MySql
        };

        public static string GetProviderName(DatabaseProvider provider)
        {
            switch (provider)
            {
                case DatabaseProvider.SQLSERVER: return "Sql Server";
                case DatabaseProvider.SQLITE: return "SQLite";
                case DatabaseProvider.ORACLE: return "Oracle";
                case DatabaseProvider.ODBC: return "ODBC";
                case DatabaseProvider.POSTGRESQL: return "PostgreSql";
                case DatabaseProvider.SQLSERVERCE: return "Sql Server CE";
                case DatabaseProvider.MySql: return "MySql";
                default: return "Sql Server";
            }
        }

        public static DatabaseProvider GetProvider(string providerName)
        {
            switch (providerName)
            {
                case "Sql Server": return DatabaseProvider.SQLSERVER;
                case "SQLite": return DatabaseProvider.SQLITE;
                case "Oracle": return DatabaseProvider.ORACLE;
                case "ODBC": return DatabaseProvider.ODBC;
                case "PostgreSql": return DatabaseProvider.POSTGRESQL;
                case "Sql Server CE": return DatabaseProvider.SQLSERVERCE;
                case "MySql": return DatabaseProvider.MySql;
                default: return DatabaseProvider.SQLSERVER;
            }
        }

        //delegates
        public delegate void ExecutedDelegate(object sender, List<SqlResult> results);
        public event ExecutedDelegate Executed;

        public string CurrentScriptPath { get; set; }

        private DbConnection _connection = null;
        private string _connectionString = null;
        private DbProviderFactory _dbFactory = null;
        private DatabaseProvider _provider = DatabaseProvider.SQLITE;
        private DBSchemaInfoBase _schemaInfo = null;
        private DbSchemaCache.DbSchemaCache _dbCache = null;
        private int _iCommandTimeout = 20;
        private object _cacheLockObj = new object();

        public SqlExecuter()
        {
            _dbCache = new SqlExecute.DbSchemaCache.DbSchemaCache();
        }

        void _completer_DebugMessage(object sender, string msg)
        {
            //TODO, propegate debug msg.
        }

        public DbConnection Connection
        {
            get { return _connection; }
        }

        public DbSchemaCache.DbSchemaCache SchemaCache
        {
            get { return _dbCache; }
        }

        public string DatabaseName => "";
        public IList<TableInfo> Tables
        {
            get
            {
                if (_dbCache == null)
                {
                    return new List<TableInfo>();
                }

                var ret = new List<TableInfo>();
                foreach (var table in _dbCache.GetTables(""))
                {
                    var ti = new TableInfo { TableName = table };
               
                    string sWhereClause = $"table_name = '{table}'";
                    _dbCache.ColumnCache.Clear();
                    _dbCache.ColumnCache.FillQuery(sWhereClause);
                    DataRow[] rows = _dbCache.ColumnCache.Select();
                    for (int r = 0; r<rows.Length; r++)
                    {
                        ti.Columns.Add( new ColumnInfo { ColumnName = (string)rows[r]["column_name"] });
                    }
                    ret.Add(ti);
                }
                return ret;
            }
        }

        public void SetTimeout(int iTimeout) //in seconds
        {
            SqlResult result = new SqlResult(SqlResult.ResultType.INFO);
            result.StartExectionTimer();

            if (iTimeout >= 0)
            {
                _iCommandTimeout = iTimeout;
                result.Message = string.Format("Command timeout set to: {0}sec", iTimeout);
            }
            else
            {
                result.Success = false;
                result.Message = "Invalid command timeout";
            }

            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> retList = new List<SqlResult>();
                retList.Add(result);
                Executed(this, retList);
            }
        }

        public void Disconnect()
        {
            SqlResult result = new SqlResult(SqlResult.ResultType.DISCONNECT);
            result.StartExectionTimer();

            try
            {
                if (_schemaInfo != null)
                {
                    _schemaInfo = null;
                }

                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                    _connectionString = null;
                    result.Message = "Disconnected OK";
                }
                result.Message = "No connection exists";
                result.Success = true;
            }
            catch (Exception ex)
            {
                if (_connection != null)
                    _connection = null;
                _connectionString = null;
                result.Success = false;
                result.Message = ex.Message;
            }

            result.StopExectionTimer();

            _dbCache.Clear();

            if (Executed != null)
            {
                List<SqlResult> retList = new List<SqlResult>();
                retList.Add(result);
                Executed(this, retList);
            }
        }

        public void Connect(string server, string database, string user, string password, DatabaseProvider provider)
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            SqlResult result = new SqlResult(SqlResult.ResultType.CONNECT);
            if (server != null)
                result.ServerName = server;
            if (database != null)
                result.DataBaseName = database;
            result.StartExectionTimer();
            
            try
            {
                switch (provider)
                {
                    case DatabaseProvider.SQLSERVER: CreateSqlServerConnection(server, database, user, password); break;
                    case DatabaseProvider.SQLITE: CreateSqliteConnection(server, database, user, password); break;
                    case DatabaseProvider.ORACLE: CreateOracleConnection(server, database, user, password); break;
                    case DatabaseProvider.ODBC: CreateODBCConnection(server); break;
                    case DatabaseProvider.POSTGRESQL: CreatePostgreSqlConnection(server, database, user, password); break;
                    case DatabaseProvider.SQLSERVERCE: CreateSqlServerCEConnection(server, database, user, password); break;
                    case DatabaseProvider.MySql: CreateMySqlConnection(server, database, user, password); break;
                    default:
                        throw new Exception("Invalid database provider, not able to create connection");
                }

                //Connect to db
                result.Message = "Connected OK";
                result.Success = true;
                
                _connection.Open();
                _schemaInfo = DBSchemaInfoBase.GetSchemaClass(provider, _connection, _dbFactory);
                GetMetaBackgroundThread();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                if (_connection != null)
                    result.Message += Environment.NewLine + "ConnectionString:" + _connection.ConnectionString;
            }
            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> retList = new List<SqlResult>();
                retList.Add(result);
                Executed(this, retList);
            }
        }

        private void GetMetaBackgroundThread()
        {
            GetSchemaInfoThread gsiThread = new GetSchemaInfoThread(_provider, _dbFactory, _connectionString);
            gsiThread.DataReady += new GetSchemaInfoThread.DataReadyDelegate(gsiThread_DataReady);

            Thread t = new Thread(gsiThread.Run);
            t.Start();
        }

        void gsiThread_DataReady(object sender, DataTable dt, long executionTimeMS)
        {
            FillCache(dt);

            SqlResult result = new SqlResult(SqlResult.ResultType.BACKGROUND_INFO);
            result.ExecutionTimeMS = executionTimeMS;
            result.Message = "Fetched Metadata OK (Background thread)";
            result.Success = true;

            if (Executed != null)
            {
                List<SqlResult> retList = new List<SqlResult>();
                retList.Add(result);
                Executed(this, retList);
            }
        }

        public void GetMetaData()
        {
            SqlResult result = new SqlResult(SqlResult.ResultType.INFO);
            result.StartExectionTimer();

            try
            {
                result.Message = "Fetched MetaData OK";
                result.Success = true;

                _schemaInfo = DBSchemaInfoBase.GetSchemaClass(_provider, _connection, _dbFactory);
                
                FillCache(null);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> retList = new List<SqlResult>();
                retList.Add(result);
                Executed(this, retList);
            }
        }

        private void FillCache(DataTable dtColumns)
        {
            lock (_cacheLockObj)
            {
                if (dtColumns == null)
                    dtColumns = GetColumnsInternal("", "");

                _dbCache.ColumnCache.TruncateTable();
                _dbCache.ColumnCache.FillQuery("");
                foreach (DataRow dr in dtColumns.Rows)
                {
                    DbSchemaCache.ColumnCacheDataRow drCol = _dbCache.ColumnCache.NewDBRow();
                    drCol.table_name = (string)dr["table_name"];
                    drCol.column_name = (string)dr["column_name"];
                    drCol.data_type = (string)dr["data_type"];
                    if (dr["column_length"] != DBNull.Value)
                        drCol.column_length = (int)dr["column_length"];
                    else
                        drCol.column_length = 0;
                    if (dr["ordinal_position"] != DBNull.Value)
                        drCol.ordinal_position = (int)dr["ordinal_position"];
                    else
                        drCol.ordinal_position = 0;

                    drCol.is_nullable = GetDBBool(dr, "is_nullable", true);
                    drCol.primary_key = GetDBBool(dr, "primary_key", false);
                    _dbCache.ColumnCache.Rows.Add(drCol);
                }
                _dbCache.ColumnCache.Save();
            }
            
        }

        private bool GetDBBool(DataRow dr, string column, bool defValue)
        {
            if (dr[column] == DBNull.Value)
                return defValue;
            return (bool)dr[column];
        }

        #region Provider specific connection string settings
        private void CreateSqliteConnection(string server, string database, string user, string password)
        {
            if (database == null || database.Length < 1)
                throw new Exception("No database file supplied to sqlite open()");

            _dbFactory = SQLiteFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = string.Format("Data Source = {0}", database);
            _connectionString = string.Format("Data Source = {0}", database);
            _provider = DatabaseProvider.SQLITE;
        }

        private void CreateSqlServerCEConnection(string server, string database, string user, string password)
        {
            if (string.IsNullOrEmpty(database))
                throw new Exception("No database file supplied to sql server CE open()");

            //_dbFactory = new SqlCeProviderFactory();
            //_connection = _dbFactory.CreateConnection();
            //SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder() {DataSource = database};
            //_connectionString = builder.ConnectionString;
            //_connection.ConnectionString = builder.ConnectionString;
            //_provider = DatabaseProvider.SQLSERVERCE;
        }

        private void CreateMySqlConnection(string server, string database, string user, string password)
        {
            MySql.Data.MySqlClient.MySqlConnectionStringBuilder mySqlCSB = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

            if (server != null && server.Length > 0)
            {
                mySqlCSB.Server = server;
            }
            if (database != null && database.Length > 0)
            {
                mySqlCSB.Database = database;
            }
            if (user != null && user.Length > 0)
            {
                mySqlCSB.UserID = user;
            }
            if (password != null && password.Length > 0)
            {
                mySqlCSB.Password = password;
            }
            else
            {
                mySqlCSB.IntegratedSecurity = true;
            }

            _dbFactory = MySql.Data.MySqlClient.MySqlClientFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = mySqlCSB.ConnectionString;
            _connectionString = mySqlCSB.ConnectionString;
            _provider = DatabaseProvider.MySql;
        }

        private void CreateSqlServerConnection(string server, string database, string user, string password)
        {
            SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();
            if (server != null && server.Length > 0)
            {
                sqlCSB.DataSource = server;
            }
            if (database != null && database.Length > 0)
            {
                sqlCSB.InitialCatalog = database;
            }
            if (user != null && user.Length > 0)
            {
                sqlCSB.UserID = user;
            }
            if (password != null && password.Length > 0)
            {
                sqlCSB.Password = password;
            }
            else
            {
                sqlCSB.IntegratedSecurity = true;
            }

            ConnectionString = sqlCSB.ConnectionString;
            _dbFactory = SqlClientFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = sqlCSB.ConnectionString;
            _connectionString = sqlCSB.ConnectionString;
            _provider = DatabaseProvider.SQLSERVER;
        }

        private void CreatePostgreSqlConnection(string server, string database, string user, string password)
        {
            //"Server=127.0.0.1;Port=5432;User Id=joe;Password=secret;Database=joedata;"
            string connectionString = ""; 
            if (server != null && server.Length > 0)
            {
                connectionString += string.Format("Server = {0};", server);
            }
            if (database != null && database.Length > 0)
            {
                connectionString += string.Format("Database = {0};", database);
            }
            if (user != null && user.Length > 0)
            {
                connectionString += string.Format("User ID = {0};", user);
            }
            if (password != null && password.Length > 0)
            {
                connectionString += string.Format("Password = {0};", password);
            }
            else
            {
                connectionString += "Integrated Security;";
            }

            ConnectionString = connectionString;
            _dbFactory = Npgsql.NpgsqlFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = connectionString;
            _connectionString = connectionString;
            _provider = DatabaseProvider.POSTGRESQL;
        }

        private void CreateProgressConnection(string server, string database, string user, string password)
        {
            string connectionString = "DRIVER=Progress OpenEdge 10.1C driver;";
            if (server != null && server.Length > 0)
            {
                connectionString += string.Format("HOST = {0};", server);
            }
            if (database != null && database.Length > 0)
            {
                connectionString += string.Format("DSN = {0};", database);
            }
            if (user != null && user.Length > 0)
            {
                connectionString += string.Format("UID = {0};", user);
            }
            if (password != null && password.Length > 0)
            {
                connectionString += string.Format("PWD = {0};", password);
            }
            else
            {
                connectionString += "Integrated Security;";
            }

            ConnectionString = connectionString;
            _dbFactory = System.Data.Odbc.OdbcFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = connectionString;
            _connectionString = connectionString;
            _provider = DatabaseProvider.ODBC;
        }
 
        private void CreateODBCConnection(string connectionString)
        {
            ConnectionString = ConnectionString;
            _dbFactory = System.Data.Odbc.OdbcFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = connectionString;
            _connectionString = connectionString;
            _provider = DatabaseProvider.ODBC;
        }

        private void CreateOracleConnection(string server, string database, string user, string password)
        {
            OracleConnectionStringBuilder oracleCSB = new OracleConnectionStringBuilder();
            if (server != null && server.Length > 0)
            {
                string tmpDataSource = server;
                if (database != null && database.Length > 0)
                    tmpDataSource += "/" + database;

                oracleCSB.DataSource = tmpDataSource;
            }
            if (user != null && user.Length > 0)
            {
                oracleCSB.UserID = user;
            }
            if (password != null && password.Length > 0)
            {
                oracleCSB.Password = password;
            }

            ConnectionString = oracleCSB.ConnectionString;
            _dbFactory = OracleClientFactory.Instance;
            _connection = _dbFactory.CreateConnection();
            _connection.ConnectionString = oracleCSB.ConnectionString;
            _connectionString = oracleCSB.ConnectionString;
            _provider = DatabaseProvider.ORACLE;
        }
        #endregion

        public void GetTables(string tableSearch)
        {
            if (!HaveConnection())
                return;

            SqlResult result = new SqlResult(SqlResult.ResultType.SCHEMA_INFO);
            result.Success = true;
            result.TableName = "SCHEMA_INFO_TABLES";
            result.StartExectionTimer();

            try
            {
                if (_schemaInfo != null)
                {
                    result.DataTable = _schemaInfo.GetTableInfo(tableSearch);
                    result.RowsAffected = result.DataTable.Rows.Count;
                }
                else if (_dbCache != null)
                {
                    List<string> tables = _dbCache.GetTables(tableSearch);
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Table Name", typeof(string));
                    foreach (string table in tables)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Table Name"] = table;
                        dt.Rows.Add(dr);
                    }
                    result.DataTable = dt;
                    result.RowsAffected = tables.Count;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> resList = new List<SqlResult>();
                resList.Add(result);
                Executed(this, resList);
            }
        }

        private DataTable GetColumnsInternal(string tableSearch, string columnsSearch)
        {
            return _schemaInfo.GetColumnsInfo(tableSearch, columnsSearch);
        }

        public List<string> GetDatabases()
        {
            return _schemaInfo.GetDatabases();
        }

        public void GetColumns(string tableSearch, string columnSearch)
        {
            if (!HaveConnection())
                return;

            SqlResult result = new SqlResult(SqlResult.ResultType.SCHEMA_INFO);
            result.Success = true;
            result.TableName = "SCHEMA_INFO_COLUMNS";
            result.StartExectionTimer();
            
            try
            {
                result.DataTable = GetColumnsInternal(tableSearch, columnSearch);
                result.RowsAffected = result.DataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> resList = new List<SqlResult>();
                resList.Add(result);
                Executed(this, resList);
            }
        }

        public void GetSchema(string schema, string[] restrictions)
        {
            if (!HaveConnection())
                return;

            SqlResult result = new SqlResult(SqlResult.ResultType.SCHEMA_INFO);
            result.Success = true;
            result.StartExectionTimer();

            try
            {
                if (schema == null)
                    result.DataTable = _schemaInfo.GetSchema();
                else if (restrictions == null || restrictions.Length < 1)
                    result.DataTable = _schemaInfo.GetSchema(schema);
                else
                    result.DataTable = _schemaInfo.GetSchema(schema, restrictions);

                result.RowsAffected = result.DataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            result.StopExectionTimer();

            if (Executed != null)
            {
                List<SqlResult> resList = new List<SqlResult>();
                resList.Add(result);
                Executed(this, resList);
            }
        }

        public bool HaveConnection()
        {
            List<SqlResult> resList = new List<SqlResult>();
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                SqlResult noConnectionResult = new SqlResult();
                noConnectionResult.Message = "No connection exists";
                noConnectionResult.Success = false;
                resList.Add(noConnectionResult);
                if (Executed != null)
                    Executed(this, resList);
                return false;
            }
            return true;
        }

        public List<SqlResult> ExecuteSql(string[] sql)
        {
            if (sql == null || sql.Length < 1)
            {
                return null;
            }

            List<SqlResult> resList = new List<SqlResult>();
            if (!HaveConnection())
                return null;

            foreach (string sqlCmd in sql)
            {
                SqlResult res = ExecuteSql(sqlCmd);
                resList.Add(res);
            }

            return resList;
        }

        public void Cancel()
        {
            _currentCommand?.Cancel();
        }

        private DbCommand _currentCommand;
        public SqlResult ExecuteSql(string sql, DbTransaction transaction = null)
        {
              
            SqlResult result = new SqlResult();
            result.Message = "Executed OK";
            result.Success = true;
            result.SqlQuery = sql;
            result.StartExectionTimer();

            if (string.IsNullOrEmpty(sql))
            {
                result.Message = "No sql command to execute!";
                result.Success = false;
                return result;
            }

            try
            {
                string realSqlCmd = null;
                if (sql[0] == ':') //text output
                {
                    realSqlCmd = sql.Substring(1, sql.Length - 1).Trim();
                    result.DisplayAsText = true;
                }
                else
                {
                    realSqlCmd = sql;
                }

                _currentCommand = _dbFactory.CreateCommand();
                _currentCommand.CommandText = realSqlCmd;
                _currentCommand.Connection = _connection;
                if (_provider != DatabaseProvider.SQLSERVERCE)
                {
                    _currentCommand.CommandTimeout = _iCommandTimeout;
                }

                if (transaction != null)
                    _currentCommand.Transaction = transaction;

                if (realSqlCmd.ToLower().IndexOf("select") == 0)
                {
                    result.DataAdapter = _dbFactory.CreateDataAdapter();
                    result.DataAdapter.SelectCommand = _currentCommand;
                    result.DataAdapter.MissingSchemaAction = MissingSchemaAction.Add;

                    DbCommandBuilder dbCommandBuilder = _dbFactory.CreateCommandBuilder();
                    dbCommandBuilder.DataAdapter = result.DataAdapter;

                    result.TableName = GetTableName(realSqlCmd);
                    result.DataTable = new DataTable();
                    result.RowsAffected = result.DataAdapter.Fill(result.DataTable);
                    result.Connection = _connection;
                }
                else
                {
                    string blobIdParam = GetBlobIdParameter(_currentCommand.CommandText);
                    if (blobIdParam != null && CurrentScriptPath != null)
                    {
                        byte[] data = File.ReadAllBytes(CurrentScriptPath + blobIdParam + ".raw");
                        if (data != null)
                        {
                            DbParameter param = _currentCommand.CreateParameter();
                            param.ParameterName = blobIdParam;
                            param.DbType = DbType.Binary;
                            param.Value = data;
                            _currentCommand.Parameters.Add(param);
                        }
                    }

                    result.RowsAffected = _currentCommand.ExecuteNonQuery();
                    result.Connection = _connection;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            finally
            {
                _currentCommand = null;
            }

            result.StopExectionTimer();
            return result;
        }

        private Regex blobRegex = new Regex("[(,]@(?<param>[a-zA-Z]+_blob_id_[a-zA-Z0-9_-]+)[,)]");
        private string GetBlobIdParameter(string sql)
        {
            Match m = blobRegex.Match(sql);
            if (m.Success)
            {
                return m.Groups["param"].Value;
            }
            return null;
        }

        private string GetTableName(string cmdText)
        {
            string tableName = "";
            Match m = Regex.Match(cmdText, @"from (?<table>\[{0,1}[a-zA-Z0-9_.]+\]{0,1})", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                tableName = m.Groups["table"].Value;
            }
            return tableName;
        }

        public void SaveResults(SqlResult result)
        {
            result.DataAdapter.Update(result.DataTable);
        }
    }
}
