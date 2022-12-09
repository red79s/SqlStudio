using System;

namespace Common.Model
{
    public class CommandHistoryItem
    {
        public long Id { get; set; }
        public string Command { get; set; }
        public DateTime LastExecuted { get; set; }
    }
}
