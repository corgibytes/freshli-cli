using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Functionality
{
    [Index(nameof(Key), IsUnique = true)]
    public class CachedProperty
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class CacheContext : DbContext
    {
        public static readonly DirectoryInfo DefaultCacheDir = new DirectoryInfo(Environment.GetEnvironmentVariable("HOME") + "/.freshli");
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

    public static class Cache
    {
        private static void MigrateIfPending(CacheContext context)
        {
            var pending = context.Database.GetPendingMigrations();
            if (pending.Any() == false)
            {
                return;
            }

            context.Database.Migrate();
        }
        public static bool Prepare(DirectoryInfo cacheDir)
        {
            CacheContext.CacheDir = cacheDir;
            Console.Out.WriteLine($"Preparing cache at {cacheDir}");

            // Create the directory if it doesn't already exist
            if (cacheDir.Exists == false)
            {
                cacheDir.Create();
            }

            using var db = new CacheContext();
            try
            {
                MigrateIfPending(db);
            }
            catch (Microsoft.Data.Sqlite.SqliteException e)
            {
                Console.Out.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
