using System.Collections.Generic;

namespace Common
{
    public interface ISqlCompleter
    {
        IList<string> GetPossibleCompletions(string sqlCommand, int index);
    }
}
