using System.Collections.Generic;

namespace Common
{
    public interface ISqlCompleter
    {
        CommandCompletionResult GetPossibleCompletions(string sqlCommand, int index);
    }
}
