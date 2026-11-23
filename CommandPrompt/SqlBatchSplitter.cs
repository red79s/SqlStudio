using System;
using System.Collections.Generic;
using System.Text;

namespace CommandPrompt
{
    public class SqlBatch
    {
        public SqlBatch(string text, int startLine, int repeatCount)
        {
            Text = text;
            StartLine = startLine;
            RepeatCount = repeatCount;
        }

        /// <summary>The batch text, sent to the server as one command.</summary>
        public string Text { get; }

        /// <summary>0-based line of the script on which the batch starts; used to map server error line numbers back to the script.</summary>
        public int StartLine { get; }

        /// <summary>How many times to run the batch ("GO 5").</summary>
        public int RepeatCount { get; }
    }

    /// <summary>
    /// Splits a script into batches on GO lines, the way SSMS and sqlcmd do. A GO inside a comment or a
    /// string does not split. Statements inside a batch are not split: DECLARE'd variables, IF/BEGIN/END
    /// blocks and transactions stay together.
    /// </summary>
    public static class SqlBatchSplitter
    {
        public static List<SqlBatch> Split(string script)
        {
            var batches = new List<SqlBatch>();
            if (string.IsNullOrEmpty(script))
                return batches;

            string[] lines = script.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            var state = SqlLexState.Normal;
            var current = new StringBuilder();
            int batchStart = 0;
            bool hasCode = false;

            for (int lineNo = 0; lineNo < lines.Length; lineNo++)
            {
                string line = lines[lineNo];

                if (state.IsNormal && SqlTokenizer.TryParseBatchSeparator(line, out _, out _, out int repeat))
                {
                    if (hasCode)
                        batches.Add(new SqlBatch(current.ToString(), batchStart, repeat));
                    current.Clear();
                    hasCode = false;
                    batchStart = lineNo + 1;
                    continue;
                }

                foreach (var token in SqlTokenizer.TokenizeLine(line, ref state))
                {
                    if (token.Type != SqlTokenType.Whitespace && token.Type != SqlTokenType.Comment)
                        hasCode = true;
                }

                if (lineNo > batchStart)
                    current.Append(Environment.NewLine);
                current.Append(line);
            }

            if (hasCode)
                batches.Add(new SqlBatch(current.ToString(), batchStart, 1));

            return batches;
        }
    }
}
