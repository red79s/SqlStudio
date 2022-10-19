using SqlExecute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlStudio.ScriptExecuter
{
    public interface IScriptExecuter
    {
        event SqlCommandExecutionFailedDelegate ExecuteFailed;
        event SqlExecuterProgressDelegate Progress;

        int NumStatementsExecuted { get; }
        int NumErrors { get; }
        TimeSpan ElapsedTime { get; }

        void Execute(SqlExecuter executer, string scriptFile);
    }
}
