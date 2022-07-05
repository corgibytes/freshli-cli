using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheContext : DbContext
{
    public const string CacheDbName = "freshli.db";

    public CacheContext(DirectoryInfo cacheDir)
    {
        CacheDir = cacheDir;
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    public static DirectoryInfo DefaultCacheDir =>
        new(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    public DirectoryInfo CacheDir { get; }
    public string DbPath { get; }

    public DbSet<CachedProperty> CachedProperties { get; set; }
    public DbSet<CachedGitRepo> CachedGitRepos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
