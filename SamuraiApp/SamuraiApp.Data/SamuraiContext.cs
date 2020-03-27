using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
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

        public static readonly ILoggerFactory MyConsoleLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // this is your connection string
            // here you database will be called SamuraiAppData
            // the first time the EFCore instantiates the samurai context at runtime, it will trigger "OnConfiguring" Method
            optionsBuilder.UseLoggerFactory(MyConsoleLoggerFactory)
                .EnableSensitiveDataLogging(true)
                .UseSqlServer("Server = (localdb)\\mssqllocaldb; Database = SamuraiAppData; Trusted_Connection = True;");
        }

        // "modelBuilder" object that EF Core pass to this function, you are telling that "SamuraiBattle" have key composed of
        // SamuraiId, and BattleId. Now EF able to build SQL Queries, and execute "update-database" that can understand this many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
        }
    }
}
