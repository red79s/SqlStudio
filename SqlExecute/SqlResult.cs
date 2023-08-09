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

        public string Message { get; set; } = "";

        public bool Success { get; set; } = true;

        private long _exectionTime = 0;
        public long ExecutionTimeMS
        {
            get { return _exectionTime; }
            set { _exectionTime = value; }
        }

        public int RowsAffected { get; set; } = 0;
        public DbDataAdapter DataAdapter { get; set; } = null;
        public string TableName { get; set; } = "";
        public string ServerName { get; set; } = "";
        public string DataBaseName { get; set; } = "";
        public bool DisplayAsText { get; set; } = false;
        public DataTable DataTable { get; set; } = null;
        public DbConnection Connection { get; set; } = null;

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
