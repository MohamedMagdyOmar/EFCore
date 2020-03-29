using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Data
{
    public class SamuraiContext:DbContext
    {
        /* public static readonly LoggerFactory MyConsoleLoggerFactory = 
             new LoggerFactory(new[] { 
             new ConsoleLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information,true
                 )});*/

        public IConfigurationRoot Configuration { get; }
        public static readonly ILoggerFactory MyConsoleLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["WPFDatabase"].ToString(); //Microsoft.Extensions.Configuration.ConfigurationExtensions.
            // this is your connection string
            // here you database will be called SamuraiAppData
            // the first time the EFCore instantiates the samurai context at runtime, it will trigger "OnConfiguring" Method
            optionsBuilder.UseLoggerFactory(MyConsoleLoggerFactory)
                .EnableSensitiveDataLogging(true)
                .UseSqlServer(connectionString);
        }

        // "modelBuilder" object that EF Core pass to this function, you are telling that "SamuraiBattle" have key composed of
        // SamuraiId, and BattleId. Now EF able to build SQL Queries, and execute "update-database" that can understand this many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
        }
    }
}
