using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality
{
    public static class Cache
    {
        private static void MigrateIfPending(CacheContext context)
        {
            var pending = context.Database.GetPendingMigrations();
            if (!pending.Any())
            {
                return;
            }

            context.Database.Migrate();
        }

        private static bool ValidateDirIsCache(DirectoryInfo cacheDir)
        {
            List<string> dirContents = cacheDir.GetFiles().Select(file => file.Name).ToList();
            // Folder is valid cache if empty or if contains "freshli.db"
            return (!dirContents.Any() || dirContents.Contains(CacheContext.CacheDbName));
        }

        public static bool Prepare(DirectoryInfo cacheDir)
        {
            CacheContext.CacheDir = cacheDir;
            Console.Out.WriteLine($"Preparing cache at {cacheDir}");

            // Create the directory if it doesn't already exist
            if (!cacheDir.Exists)
            {
                cacheDir.Create();
            }
            else if (!ValidateDirIsCache(cacheDir))
            {
                Console.Out.WriteLine($"We cannot use an existing non-empty directory as a cache directory.");
                return false;
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

        public static bool Destroy(DirectoryInfo cacheDir)
        {
            // If the directory doesn't exist, do nothing (be idempotent).
            if (!cacheDir.Exists)
            {
                Console.Error.WriteLine("Cache directory already destroyed or does not exist.");
                return true;
            }

            if (!ValidateDirIsCache(cacheDir))
            {
                Console.Out.WriteLine($"This directory is not a Freshli cache. Directory not destroyed.");
                return false;
            }

            cacheDir.Delete(true);
            return true;
        }
    }
}
