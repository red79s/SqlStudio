using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using SqlExecute;
using CfgDataStore;

namespace SqlStudio
{
    class Executer
    {
        public delegate void ExecutionFinishedDelegate(object sender, List<SqlResult> results);
        public event ExecutionFinishedDelegate ExecutionFinished;

        private SqlExecuter _sqlExecuter = null;
        public SqlExecuter SqlExecuter
        {
            get { return _sqlExecuter; }
            set { _sqlExecuter = value; }
        }

        private bool _bussy = false;
        private Thread _thread = null;
        private string[] _currentCommandLines = null;
        private CommandPrompt.CmdLineControl _cmdControl = null;
        private ConfigDataStore _cfgDataStore = null;
        public DbConnectionInfo CurrentConnection { get; private set; }

        public Executer(CommandPrompt.CmdLineControl cmdControl, ConfigDataStore cfgDataStore)
        {
            _cmdControl = cmdControl;
            _cfgDataStore = cfgDataStore;
            _sqlExecuter = new SqlExecuter();
            _sqlExecuter.Executed += new SqlExecuter.ExecutedDelegate(_sqlExecuter_Executed);
        }

        public List<string> GetPosibleCompletions(string sqlCmd, int index)
        {
            return _sqlExecuter.GetPosibleCompletions(sqlCmd, index);
        }

        public string CurrentScriptPath 
        {
            get
            {
                return _sqlExecuter.CurrentScriptPath;
            }
            set
            {
                _sqlExecuter.CurrentScriptPath = value;
            }
        }

        public SqlExecute.DbSchemaCache.DbSchemaCache SchemaCache
        {
            get
            {
                lock (this)
                {
                    if (_bussy)
                        return null;

                    return _sqlExecuter.SchemaCache;
                }
            }
        }
        
        public List<string> GetDatabases()
        {
            return _sqlExecuter.GetDatabases();
        }

        public DbConnection Connection
        {
            get
            {
                if (_sqlExecuter != null && _sqlExecuter.Connection != null)
                {
                    return _sqlExecuter.Connection;
                }
                return null;
            }
        }

        public string GetConnectionString()
        {
            if (_sqlExecuter != null && _sqlExecuter.Connection != null)
            {
                return _sqlExecuter.Connection.ConnectionString;
            }
            return null;
        }

        public void Execute(string[] cmdLines)
        {
            lock (this)
            {
                if (_bussy)
                    throw new Exception("Already executing");
                _bussy = true;
            }

            _currentCommandLines = cmdLines;
            _thread = new Thread(new ThreadStart(Execute));
            _thread.IsBackground = false;
            _thread.Start();
        }

        public bool IsBussy
        {
            get
            {
                lock (this)
                {
                    return _bussy;
                }
            }
        }

        public void Cancel()
        {
            _sqlExecuter.Cancel();
        }

        public void CancelExecution()
        {
            lock (this)
            {
                if (!_bussy)
                    return;
                if (_thread == null)
                    return;
                if (_thread.IsAlive && _thread.ThreadState == ThreadState.Running)
                    _thread.Abort();
                _bussy = false;
            }
        }

        public void Execute()
        {
            try
            {
                bool bTranselated = false;
                if (_currentCommandLines.Length < 2)
                {
                    ArgumentsParser args = new ArgumentsParser(_currentCommandLines[0]);
                    if (args.GetCommand() == "connect")
                    {
                        SqlExecuter.DatabaseProvider provider = SqlExecuter.DatabaseProvider.SQLSERVER;
                        string[] nonNamed = args.GetNonNamedArgs();
                        if (nonNamed.Length > 0)
                        {
                            switch (nonNamed[0])
                            {
                                case "SQLite": provider = SqlExecuter.DatabaseProvider.SQLITE; break;
                                case "Oracle": provider = SqlExecuter.DatabaseProvider.ORACLE; break;
                                case "ODBC": provider = SqlExecuter.DatabaseProvider.ODBC; break;
                                case "Sql Server": provider = SqlExecuter.DatabaseProvider.SQLSERVER; break;
                                case "PostgreSql": provider = SqlExecuter.DatabaseProvider.POSTGRESQL; break;
                                case "Sql Server CE": provider = SqlExecuter.DatabaseProvider.SQLSERVERCE; break;
                                case "MySql": provider = SqlExecuter.DatabaseProvider.MySql; break;
                                default: throw new Exception(string.Format("Unknown provider: {0}", nonNamed[0]));
                            }
                        }

                        string server = args.GetNamedArg("s");
                        string db = args.GetNamedArg("d");
                        string user = args.GetNamedArg("u");
                        string password = args.GetNamedArg("p");

                        CurrentConnection = new DbConnectionInfo { ProviderName = nonNamed[0], Server = server, Database = db, User = user, Password = password };

                        _sqlExecuter.Connect(server, db, user, password, provider);
                        bTranselated = true;
                    }
                    else if (args.GetCommand() == "disconnect")
                    {
                        CurrentConnection = null;
                        _sqlExecuter.Disconnect();
                        bTranselated = true;
                    }
                    else if (args.GetCommand() == "getschema")
                    {
                        string schema = null;
                        string[] restrictions = null;
                        if (args.NumNonNamedArgs > 0)
                            schema = args.GetNonNamedArg(0);
                        if (args.NumNonNamedArgs > 1)
                        {
                            restrictions = new string[args.NumNonNamedArgs - 1];
                            for (int i = 1; i < args.NumNonNamedArgs; i++)
                            {
                                if (args.GetNonNamedArg(i) == "")
                                    restrictions[i - 1] = null;
                                else
                                    restrictions[i - 1] = args.GetNonNamedArg(i);
                            }
                        }
                        _sqlExecuter.GetSchema(schema, restrictions);
                        bTranselated = true;
                    }
                    else if (args.GetCommand() == "meta")
                    {
                        _sqlExecuter.GetMetaData();
                        bTranselated = true;
                    }
                    else if (args.GetCommand() == "set")
                    {
                        if (args.NumNonNamedArgs > 1)
                        {
                            if (args.GetNonNamedArg(0) == "timeout")
                            {
                                int iTimeout = -1;
                                int.TryParse(args.GetNonNamedArg(1), out iTimeout);
                                _sqlExecuter.SetTimeout(iTimeout);
                                bTranselated = true;
                            }
                        }
                    }
                    else if (args.GetCommand() == "ls")
                    {
                        string[] nonNamed = args.GetNonNamedArgs();
                        if (args.FlagIsSet('t'))
                        {
                            string tableSearch = "";
                            if (nonNamed.Length > 0)
                                tableSearch = InsertPercentageSearch(nonNamed[0]);
                            _sqlExecuter.GetTables(tableSearch);
                        }
                        else if (args.FlagIsSet('c'))
                        {
                            string tableSearch = "";
                            if (nonNamed.Length > 0)
                                tableSearch = InsertPercentageSearch(nonNamed[0]);
                            string columnSearch = "";
                            if (nonNamed.Length > 1)
                                columnSearch = InsertPercentageSearch(nonNamed[1]);

                            _sqlExecuter.GetColumns(tableSearch, columnSearch);
                        }
                        else
                        {
                            _sqlExecuter.GetSchema(null, null);
                        }
                        bTranselated = true;
                    }
                }

                if (!bTranselated)
                {
                    var res = SqlExecuter.ExecuteSql(_currentCommandLines);
                    if (ExecutionFinished != null)
                    {
                        ExecutionFinished(this, res);
                    }
                    lock (this)
                    {
                        _bussy = false;
                    }
                }
            }
            catch (Exception ex)
            {
                List<SqlResult> resList = new List<SqlResult>();
                SqlResult result = new SqlResult();
                result.Success = false;
                result.Message = ex.Message;

                resList.Add(result);
                if (ExecutionFinished != null)
                    ExecutionFinished(this, resList);
                
                lock (this)
                {
                    _bussy = false;
                }
            }
        }

        public void LockExecuter()
        {
            lock (this)
            {
                if (_bussy)
                    throw new Exception("Exucter is already locked");
                _bussy = true;
            }
        }

        public void UnlockExecuter()
        {
            lock (this)
            {
                if (!_bussy)
                    throw new Exception("Executer wasn't locked");
                _bussy = false;
            }
        }

        private string InsertPercentageSearch(string input)
        {
            if (!input.Contains("%"))
                return "%" + input + "%";
            return input;
        }

        void _sqlExecuter_Executed(object sender, List<SqlExecute.SqlResult> results)
        {
            if (ExecutionFinished != null)
            {
                ExecutionFinished(this, results);
            }

            lock (this)
            {
                _bussy = false;
            }
        }

        public void ExecuteScriptFile(string fileName)
        {
        }
    }
}
