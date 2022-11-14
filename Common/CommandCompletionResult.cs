using System.Collections.Generic;

namespace Common
{
    public class CommandCompletionResult
    {
        public string CompletedText { get; set; } = "";
        public int CompletedTextStartIndex { get; set; } = 0;
        public IList<string> PossibleCompletions { get; set; } = new List<string>();
    }
}
