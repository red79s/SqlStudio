using System;

namespace CfgDataStore
{
    public class HistoryLogItem
    {
        public long Id { get; set; }
        public string Command { get; set; }
        public DateTime LastExecuted { get; set; }
    }
}
