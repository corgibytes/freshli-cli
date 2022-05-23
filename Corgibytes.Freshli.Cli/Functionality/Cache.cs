using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality
{
    [Serializable]
    public class CacheException : Exception
    {
        public CacheException(string message, Exception innerException) : base(message, innerException) {}
        public CacheException(string message) : base(message) {}
        protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    [Serializable]
    public class CacheWarningException : WarningException
    {
        public CacheWarningException(string message, Exception innerException) : base(message, innerException) {}

        public CacheWarningException(string message) : base(message) {}
        protected CacheWarningException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

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

            // Create the directory if it doesn't already exist
            if (!cacheDir.Exists)
            {
                cacheDir.Create();
            }
            else if (!ValidateDirIsCache(cacheDir))
            {
                throw new CacheException($"We cannot use an existing non-empty directory as a cache directory.");
            }

            using var db = new CacheContext();
            try
            {
                MigrateIfPending(db);
            }
            catch (Microsoft.Data.Sqlite.SqliteException e)
            {
                throw new CacheException(e.Message, e);
            }

            return true;
        }

        public static bool Destroy(DirectoryInfo cacheDir)
        {
            // If the directory doesn't exist, do nothing (be idempotent).
            if (!cacheDir.Exists)
            {
                throw new CacheWarningException("Cache directory already destroyed or does not exist.");
            }

            if (!ValidateDirIsCache(cacheDir))
            {
                throw new CacheException($"This directory is not a Freshli cache. Directory not destroyed.");
            }

            cacheDir.Delete(true);
            return true;
        }
    }
}
