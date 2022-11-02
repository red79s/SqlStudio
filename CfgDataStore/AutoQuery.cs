using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CfgDataStore
{
    [Table("AutoQueries")]
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
