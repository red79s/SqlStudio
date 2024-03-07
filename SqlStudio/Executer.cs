using CfgDataStore;
using Common;
using Common.Model;
using SqlExecute;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlStudio
{
    public class Executer : IExecuter
    {
        public EventHandler<IList<SqlResult>> ExecutionFinished;

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
        private readonly IDatabaseKeywordEscape _databaseKeywordEscape;
        private readonly ILogger _logger;

        public DbConnectionInfo CurrentConnection { get; private set; }

        public Executer(CommandPrompt.CmdLineControl cmdControl, ConfigDataStore cfgDataStore, IDatabaseKeywordEscape databaseKeywordEscape, ILogger logger)
        {
            _logger = logger;
            _cmdControl = cmdControl;
            _cfgDataStore = cfgDataStore;
            _databaseKeywordEscape = databaseKeywordEscape;
            _sqlExecuter = new SqlExecuter(_logger, _databaseKeywordEscape);
            _sqlExecuter.Executed += _sqlExecuter_Executed;
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

        public Task DeleteCascading(TableKeyValues tableKeyValues, bool onlyDisplayAffectedRows)
        {
            return Task.Run(() =>
            {
                var res = _sqlExecuter.DeleteCascading(tableKeyValues, onlyDisplayAffectedRows);
                ExecutionFinished?.Invoke(this, res);
            });
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

        private void _sqlExecuter_Executed(object sender, IList<SqlResult> results)
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

        public SqlResult Execute(string sqlQuerys)
        {
            return SqlExecuter.ExecuteSql(sqlQuerys);
        }
    }
}
