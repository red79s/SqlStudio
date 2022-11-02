// class ConnectionsDataRow
// Automaticaly generated file: 12/17/2006 6:42:47 PM

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
	[Table("connections")]
	public class Connection
	{
		[Key]
		public long p_key { get; set; }

		public string provider { get; set; }

		public string server { get; set; }

		public string db { get; set; }

		public string user { get; set; }

		public string password { get; set; }

		public bool integrated_security { get; set; }

        public bool default_connection { get; set; }

		public string description { get; set; }
	}
}
