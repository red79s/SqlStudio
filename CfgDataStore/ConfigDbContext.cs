using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;

namespace CfgDataStore
{
    public class ConfigDbContext : DbContext
    {
        private readonly string _configFile;

        public ConfigDbContext(string configFile)
            : base(new SQLiteConnection($"data source = {configFile}"), true)
        {
            _configFile = configFile;
            EnusureTablesExists();
        }

        public DbSet<Connection> Connections { get; set; }
        public DbSet<CfgValue> Cfg_Values { get; set; }
        public DbSet<HistoryItem> History { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<AutoQuery> AutoQueries { get;set;}

        public void EnusureTablesExists()
        {
            try
            {
                Database.ExecuteSqlCommand("Select count (*) from AutoQueries");
            }
            catch
            {
                Database.ExecuteSqlCommand("create table AutoQueries(AutoQueryId INTEGER primary key , description nvarchar(255), tablename nvarchar(255), columnname nvarchar(255) , command nvarchar(5000));");
            }
        }
    }

    public class SqliteDbConfiguration : DbConfiguration
    {
        public SqliteDbConfiguration()
        {
            string assemblyName = typeof(SQLiteProviderFactory).Assembly.GetName().Name;

            RegisterDbProviderFactories(assemblyName);
            SetProviderFactory(assemblyName, SQLiteFactory.Instance);
            SetProviderFactory(assemblyName, SQLiteProviderFactory.Instance);
            SetProviderServices(assemblyName,
                (DbProviderServices)SQLiteProviderFactory.Instance.GetService(
                    typeof(DbProviderServices)));
        }

        static void RegisterDbProviderFactories(string assemblyName)
        {
            var dataSet = ConfigurationManager.GetSection("system.data") as DataSet;
            if (dataSet != null)
            {
                var dbProviderFactoriesDataTable = dataSet.Tables.OfType<DataTable>()
                    .First(x => x.TableName == typeof(DbProviderFactories).Name);

                var dataRow = dbProviderFactoriesDataTable.Rows.OfType<DataRow>()
                    .FirstOrDefault(x => x.ItemArray[2].ToString() == assemblyName);

                if (dataRow != null)
                    dbProviderFactoriesDataTable.Rows.Remove(dataRow);

                dbProviderFactoriesDataTable.Rows.Add(
                    "SQLite Data Provider (Entity Framework 6)",
                    ".NET Framework Data Provider for SQLite (Entity Framework 6)",
                    assemblyName,
                    typeof(SQLiteProviderFactory).AssemblyQualifiedName
                    );
            }
        }
    }
}
