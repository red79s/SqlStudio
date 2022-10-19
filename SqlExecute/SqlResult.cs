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
            this._resultType = resultType;
        }

        public SqlResult()
        {
            this._resultType = ResultType.UNKNOWN;
        }

        private ResultType _resultType = ResultType.UNKNOWN;
        public ResultType ResType
        {
            get { return this._resultType; }
            set { this._resultType = value; }
        }

        private string _message = "";
        public string Message
        {
            get { return this._message; }
            set { this._message = value; }
        }

        private bool _success = true;
        public bool Success
        {
            get { return this._success; }
            set { this._success = value; }
        }

        private long _exectionTime = 0;
        public long ExecutionTimeMS
        {
            get { return this._exectionTime; }
            set { this._exectionTime = value; }
        }

        private int _rowsAffected = 0;
        public int RowsAffected
        {
            get { return this._rowsAffected; }
            set { this._rowsAffected = value; }
        }

        private DbDataAdapter _dbDataAdapter = null;
        public DbDataAdapter DataAdapter
        {
            get { return this._dbDataAdapter; }
            set { this._dbDataAdapter = value; }
        }

        private string _tableName = "";
        public string TableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }

        private string _serverName = "";
        public string ServerName
        {
            get { return this._serverName; }
            set { this._serverName = value; }
        }

        private string _databaseName = "";
        public string DataBaseName
        {
            get { return this._databaseName; }
            set { this._databaseName = value; }
        }

        private bool _displayAsText = false;
        public bool DisplayAsText
        {
            get { return this._displayAsText; }
            set { this._displayAsText = value; }
        }

        private DataTable _dtTable = null;
        public DataTable DataTable
        {
            get { return this._dtTable; }
            set { this._dtTable = value; }
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
            if (this._sw == null)
            {
                this._sw = new System.Diagnostics.Stopwatch();
            }
            this._sw.Start();
        }

        public void StopExectionTimer()
        {
            if (this._sw != null && this._sw.IsRunning)
            {
                this._sw.Stop();
                this._exectionTime = this._sw.ElapsedMilliseconds;
            }
        }
    }
}
