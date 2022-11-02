// class AliasesDataRow
// Automaticaly generated file: 12/17/2006 5:28:54 PM

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
	[Table("History")]
    public class HistoryItem
	{
		[Key]
		public long p_key { get; set; }

		public string command { get; set; }
	}
}
