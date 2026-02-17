using Microsoft.EntityFrameworkCore;

namespace CfgDataStore
{
    public class ConfigDbContext : DbContext
    {
        private readonly string _configFile;

        public ConfigDbContext(string configFile)
            : base()
        {
            _configFile = configFile;
            EnusureTablesExists();
            EnsureColumnsExists();
        }

        public DbSet<Connection> Connections { get; set; }
        public DbSet<CfgValue> Cfg_Values { get; set; }
        public DbSet<HistoryItem> History { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<AutoQuery> AutoQueries { get;set;}
        public DbSet<HistoryLogItem> HistoryLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_configFile}");

        public void EnusureTablesExists()
        {
            EnsureTableExists("AutoQueries", "AutoQueryId INTEGER primary key , description nvarchar(255), tablename nvarchar(255), columnname nvarchar(255) , command nvarchar(5000)");
            EnsureTableExists("HistoryLogItems", "Id INTEGER Primary key, command nvarchar(1000), LastExecuted DateTime");
        }

        public void EnsureColumnsExists()
        {
            EnsureColumnExists("Connections", "IsProduction", "boolean");
        }

        public bool EnsureColumnExists(string tablename, string columnName, string columnType)
        {
            try
            {
                var sql = $"Select {columnName} from {tablename}";
                Database.ExecuteSqlRaw(sql);
                return true;
            }
            catch
            {
                var sql = $"alter table {tablename} add column {columnName} {columnType}";
                Database.ExecuteSqlRaw(sql);
                return false;
            }
        }

        public void EnsureTableExists(string tablename, string tableCreateSql)
        {
            try
            {
                var sql = $"Select count (*) from {tablename}";
                Database.ExecuteSqlRaw(sql);
            }
            catch
            {
                var sql = $"create table {tablename}({tableCreateSql})";
                Database.ExecuteSqlRaw(sql);
            }
        }
    }
}
