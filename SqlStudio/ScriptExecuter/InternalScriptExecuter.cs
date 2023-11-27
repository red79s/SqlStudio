using Common.Model;
using SqlExecute;
using System;
using System.Diagnostics;

namespace SqlStudio.ScriptExecuter
{
    public class InternalScriptExecuter : IScriptExecuter
    {
        public event SqlCommandExecutionFailedDelegate ExecuteFailed;
        public event SqlExecuterProgressDelegate Progress;

        public int NumStatementsExecuted { get; private set; }
        public int NumErrors { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }

        private const int REPORT_PROGRESS_ROWS = 10;

        private IScriptReader _scriptReader;
        public InternalScriptExecuter(IScriptReader scriptReader)
        {
            _scriptReader = scriptReader;
        }

        public void Execute(SqlExecuter executer, string scriptFile)
        {
            _scriptReader.SetFile(scriptFile);
            NumStatementsExecuted = 0;
            NumErrors = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var transaction = executer.Connection.BeginTransaction();

            String line = null;
            while ((line = _scriptReader.NextStatement()) != null)
            {
                SqlResult res = executer.ExecuteSql(line, transaction);
                if (!res.Success)
                {
                    if (ExecuteFailed != null)
                        ExecuteFailed(this, line, res.Message, _scriptReader.GetCurrentLine());
                    NumErrors++;
                }
                NumStatementsExecuted++;
                if (NumStatementsExecuted % 100 == 0)
                {
                    transaction.Commit();
                    transaction = executer.Connection.BeginTransaction();
                }
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
