using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
    [Table("HistoryLogItems")]
    public class HistoryLogItem
    {
        public long Id { get; set; }
        public string Command { get; set; }
        public DateTime LastExecuted { get; set; }
    }
}
