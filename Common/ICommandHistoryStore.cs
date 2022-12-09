using Common.Model;
using System.Collections.Generic;

namespace Common
{
    public interface ICommandHistoryStore
    {
        int MaxHistoryItems { get; }
        void ClearHistory();
        List<CommandHistoryItem> GetHistoryItems();
        void SetHistoryItems(List<string> items);
        void AddHistoryItem(string command);
    }
}
