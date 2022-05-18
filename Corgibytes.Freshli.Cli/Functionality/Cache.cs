using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality
{
    public static class Cache
    {
        private static readonly List<string> s_knownCacheFiles = new()
        {
            "freshli.db",
        };

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

        public static bool Destroy(DirectoryInfo cacheDir)
        {
            // If the directory doesn't exist, do nothing (be idempotent).
            if (cacheDir.Exists == false)
            {
                Console.Error.WriteLine("Cache directory already destroyed or does not exist.");
                return true;
            }

            // Find and delete each known cache file in the cache directory.
            List<FileInfo> filesInCacheDir = cacheDir.GetFiles().ToList();

            List<FileInfo> toDelete =
                (from file in filesInCacheDir where s_knownCacheFiles.Contains(file.Name) select file).ToList();

            // If no known cache files were found in the non-empty cache directory, fail with warning message.
            if (toDelete.Any() == false && filesInCacheDir.Any())
            {
                Console.Error.WriteLine("Provided --cache-dir contained no known Freshli cache files. Directory not destroyed.");
                return false;
            }

            // Delete each cache file from the directory.
            foreach (var file in toDelete)
                file.Delete();

            // Delete the cache directory if it is now empty.
            if (cacheDir.GetFiles().Length == 0)
                cacheDir.Delete();
            else
                Console.Error.WriteLine("Cache directory contains files not belonging to Freshli. Directory not destroyed.");

            return true;
        }
    }
}
