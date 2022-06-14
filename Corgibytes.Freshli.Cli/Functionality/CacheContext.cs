using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality
{
    public class CacheContext : DbContext
    {
        public static readonly DirectoryInfo DefaultCacheDir =
            new DirectoryInfo(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

        public static DirectoryInfo CacheDir = DefaultCacheDir;

        public static readonly string CacheDbName = "freshli.db";
        public string DbPath { get; }

        public DbSet<CachedProperty> CachedProperties { get; set; }

        public CacheContext()
        {
            DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
