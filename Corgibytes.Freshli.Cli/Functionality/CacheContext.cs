using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheContext : DbContext
{
    public static DirectoryInfo DefaultCacheDir =>
        new(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    public DirectoryInfo CacheDir { get; }

    public const string CacheDbName = "freshli.db";
    public string DbPath { get; }

    public DbSet<CachedProperty> CachedProperties { get; set; }

    public CacheContext(DirectoryInfo cacheDir)
    {
        CacheDir = cacheDir;
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
