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
        public static readonly string CacheDirEnvVariable = "FRESHLI_CACHE_DIR";
        public static readonly DirectoryInfo CacheDirDefault = new DirectoryInfo(Environment.GetEnvironmentVariable("HOME") + "/.freshli");
        private static readonly string cacheDbName = "freshli.db";
        public string DbPath { get; }

        public static string CacheDir
        {
            get
            {
                /* If the user passes --cache-dir, the environment variable is set. This design enables 'dotnet ef'
                 * to keep working, even after a call to `cache prepare --cache-dir [some directory]`.
                 * Otherwise, the default is used. */
                string cacheDir = Environment.GetEnvironmentVariable(CacheDirEnvVariable);
                return cacheDir.IsNullOrEmpty() ? CacheDirDefault.ToString() : cacheDir;
            }
        }

        public DbSet<CachedProperty> CachedProperties { get; set; }

        public CacheContext()
        {
            DbPath = Path.Join(CacheDir, cacheDbName);
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
            // Create the directory if it doesn't already exist
            if (cacheDir.Exists == false)
            {
                cacheDir.Create();
            }

            /* Store the expected cache directory in an environment variable so it can be used by the database model,
             * even between calls in this terminal session.
             */
            Environment.SetEnvironmentVariable(CacheContext.CacheDirEnvVariable, cacheDir.ToString());

            Console.Out.WriteLine($"Preparing cache at {CacheContext.CacheDir}");

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
