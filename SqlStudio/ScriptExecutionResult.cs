using Common.Model;
using System.Collections.Generic;

namespace SqlStudio
{
    /// <summary>The outcome of running a script from the script editor.</summary>
    public class ScriptExecutionResult
    {
        public List<SqlResult> ResultSets { get; } = new List<SqlResult>();

        /// <summary>PRINT output, row counts, errors and the completion summary, in order.</summary>
        public string Messages { get; set; } = "";

        public bool HasErrors { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
