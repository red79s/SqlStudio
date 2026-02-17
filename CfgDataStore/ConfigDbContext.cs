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
                Database.ExecuteSql($"Select {columnName} from {tablename}");
                return true;
            }
            catch
            {
                Database.ExecuteSql($"alter table {tablename} add column {columnName} {columnType}");
                return false;
            }
        }

        public void EnsureTableExists(string tablename, string tableCreateSql)
        {
            try
            {
                Database.ExecuteSql($"Select count (*) from {tablename}");
            }
            catch
            {
                Database.ExecuteSql($"create table {tablename}({tableCreateSql})");
            }
        }
    }

    //public class SqliteDbConfiguration : DbConfiguration
    //{
    //    public SqliteDbConfiguration()
    //    {
    //        string assemblyName = typeof(SQLiteProviderFactory).Assembly.GetName().Name;

    //        RegisterDbProviderFactories(assemblyName);
    //        SetProviderFactory(assemblyName, SQLiteFactory.Instance);
    //        SetProviderFactory(assemblyName, SQLiteProviderFactory.Instance);
    //        SetProviderServices(assemblyName,
    //            (DbProviderServices)SQLiteProviderFactory.Instance.GetService(
    //                typeof(DbProviderServices)));
    //    }

    //    static void RegisterDbProviderFactories(string assemblyName)
    //    {
    //        var dataSet = ConfigurationManager.GetSection("system.data") as DataSet;
    //        if (dataSet != null)
    //        {
    //            var dbProviderFactoriesDataTable = dataSet.Tables.OfType<DataTable>()
    //                .First(x => x.TableName == typeof(DbProviderFactories).Name);

    //            var dataRow = dbProviderFactoriesDataTable.Rows.OfType<DataRow>()
    //                .FirstOrDefault(x => x.ItemArray[2].ToString() == assemblyName);

    //            if (dataRow != null)
    //                dbProviderFactoriesDataTable.Rows.Remove(dataRow);

    //            dbProviderFactoriesDataTable.Rows.Add(
    //                "SQLite Data Provider (Entity Framework 6)",
    //                ".NET Framework Data Provider for SQLite (Entity Framework 6)",
    //                assemblyName,
    //                typeof(SQLiteProviderFactory).AssemblyQualifiedName
    //                );
    //        }
    //    }
    //}
}
