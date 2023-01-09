using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace SqlExecute
{
    public class SqlResult
    {
        public enum ResultType { CONNECT, DISCONNECT, SELECT, UPDATE, INSERT, DELETE, SCHEMA_INFO, INFO, UNKNOWN, BACKGROUND_INFO };

        private System.Diagnostics.Stopwatch _sw = null;
        public SqlResult(ResultType resultType)
        {
            _resultType = resultType;
        }

        public SqlResult()
        {
            _resultType = ResultType.UNKNOWN;
        }

        private ResultType _resultType = ResultType.UNKNOWN;
        public ResultType ResType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private bool _success = true;
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        private long _exectionTime = 0;
        public long ExecutionTimeMS
        {
            get { return _exectionTime; }
            set { _exectionTime = value; }
        }

        private int _rowsAffected = 0;
        public int RowsAffected
        {
            get { return _rowsAffected; }
            set { _rowsAffected = value; }
        }

        private DbDataAdapter _dbDataAdapter = null;
        public DbDataAdapter DataAdapter
        {
            get { return _dbDataAdapter; }
            set { _dbDataAdapter = value; }
        }

        private string _tableName = "";
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        private string _serverName = "";
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        private string _databaseName = "";
        public string DataBaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        private bool _displayAsText = false;
        public bool DisplayAsText
        {
            get { return _displayAsText; }
            set { _displayAsText = value; }
        }

        private DataTable _dtTable = null;
        public DataTable DataTable
        {
            get { return _dtTable; }
            set { _dtTable = value; }
        }

        private DbConnection _connection = null;
        public DbConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public string SqlQuery
        {
            get;
            set;
        }

        public void StartExectionTimer()
        {
            if (_sw == null)
            {
                _sw = new System.Diagnostics.Stopwatch();
            }
            _sw.Start();
        }

        public void StopExectionTimer()
        {
            if (_sw != null && _sw.IsRunning)
            {
                _sw.Stop();
                _exectionTime = _sw.ElapsedMilliseconds;
            }
        }
    }
}
