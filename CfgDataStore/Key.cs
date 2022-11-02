// class KeysDataRow
// Automaticaly generated file: 12/17/2006 5:25:47 PM

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
	[Table("keys")]
	public class Key
	{
		[Key]
		public long p_key { get; set; }

		public string table_name { get; set; }
		public long current_key { get; set; }
	}
}
