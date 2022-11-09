// class AliasesDataRow
// Automaticaly generated file: 12/17/2006 5:28:54 PM

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
	[Table("aliases")]
	public class Alias 
	{
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long p_key { get; set; }

		public string alias_name { get; set; }

		public string alias_value { get; set; }
	}
}
