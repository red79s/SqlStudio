using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CfgDataStore
{
    public class AutoQuery
    {
        [Key]
        public long AutoQueryId { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Command { get; set; }
    }
}
