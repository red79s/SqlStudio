using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CfgDataStore
{
    public class ConfigDbContext : DbContext
    {
        private readonly string _configFile;

        public ConfigDbContext(string configFile)
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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={_configFile}");

        public void EnusureTablesExists()
        {
            try
            {
                Database.ExecuteSqlRaw("Select count (*) from AutoQueries");
            }
            catch
            {
                Database.ExecuteSqlRaw("create table AutoQueries(AutoQueryId INTEGER primary key , description nvarchar(255), tablename nvarchar(255), columnname nvarchar(255) , command nvarchar(5000));");
            }
        }
    }
}
