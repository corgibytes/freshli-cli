using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality
{
    public class CacheContext : DbContext
    {
        public static readonly DirectoryInfo DefaultCacheDir =
            new(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

        public static DirectoryInfo CacheDir = DefaultCacheDir;

        private static readonly string cacheDbName = "freshli.db";
        public string DbPath { get; }

        public DbSet<CachedProperty> CachedProperties { get; set; }

        public CacheContext()
        {
            DbPath = Path.Join(CacheDir.ToString(), cacheDbName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
