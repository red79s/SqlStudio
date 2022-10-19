using SqlExecute;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.ScriptExecuter
{
    public class MsSqlScriptExecuter : IScriptExecuter
    {
        public event SqlCommandExecutionFailedDelegate ExecuteFailed;
        public event SqlExecuterProgressDelegate Progress;

        public int NumStatementsExecuted { get; private set; }
        public int NumErrors { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        private const int REPORT_PROGRESS_ROWS = 10;
        private readonly IScriptReader _scriptReader;

        public MsSqlScriptExecuter(IScriptReader scriptReader)
        {
            _scriptReader = scriptReader;
        }
        public void Execute(SqlExecuter executer, string scriptFile)
        {
            NumStatementsExecuted = 0;
            NumErrors = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var transaction = executer.Connection.BeginTransaction();

            _scriptReader.SetFile(scriptFile);

            String cmd = null;
            while ((cmd = _scriptReader.NextStatement()) != null)
            {
                if (cmd == "GO")
                {
                    transaction.Commit();
                    transaction = executer.Connection.BeginTransaction();
                    continue;
                }

                SqlResult res = executer.ExecuteSql(cmd, transaction);
                if (!res.Success)
                {
                    if (ExecuteFailed != null)
                        ExecuteFailed(this, cmd, res.Message, _scriptReader.GetCurrentLine());
                    NumErrors++;

                    transaction.Rollback();
                    transaction = executer.Connection.BeginTransaction();
                }
                NumStatementsExecuted++;
                
                if (NumStatementsExecuted % REPORT_PROGRESS_ROWS == 0)
                {
                    if (Progress != null)
                        Progress(this, NumStatementsExecuted);
                }
            }
            
            transaction.Commit();
            sw.Stop();
            ElapsedTime = sw.Elapsed;
        }
    }
}
