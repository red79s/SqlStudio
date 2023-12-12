using Common.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SqlExecute
{
    public class GetSchemaInfoThread
    {
        public delegate void DataReadyDelegate(object sender, DataTable dt, List<ForeignKeyInfo> foreignKeyInfos, long executionTimeMS);
        public event DataReadyDelegate DataReady;

        private DbProviderFactory _factory = null;
        private string _connectionString = null;
        SqlExecute.SqlExecuter.DatabaseProvider _provider = SqlExecuter.DatabaseProvider.ODBC;

        public GetSchemaInfoThread(SqlExecute.SqlExecuter.DatabaseProvider provider, DbProviderFactory factory, string connectionString)
        {
            _provider = provider;
            _factory = factory;
            _connectionString = connectionString;
        }

        public void Run()
        {
            DataTable dt = null;
            long executionTime = 0;
            List<ForeignKeyInfo> foreignKeyInfos = null;

            DbConnection con = _factory.CreateConnection();
            con.ConnectionString = _connectionString.Trim();
            con.Open();

            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                DBSchemaInfoBase schema = DBSchemaInfoBase.GetSchemaClass(_provider, con, _factory);
                dt = schema.GetColumnsInfo("", "");
                foreignKeyInfos = schema.GetForeignKeyInfo();
                sw.Stop();
                executionTime = sw.ElapsedMilliseconds;
            }
            finally
            {
                con.Close();
            }
            if (dt != null && DataReady != null)
                DataReady(this, dt, foreignKeyInfos, executionTime);
        }
    }
}
