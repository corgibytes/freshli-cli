using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

public class CacheContext : DbContext
{
    public const string CacheDbName = "freshli.db";

    public CacheContext(DirectoryInfo cacheDir)
    {
        CacheDir = cacheDir;
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    // ReSharper disable once UnusedMember.Global
    public static DirectoryInfo DefaultCacheDir =>
        new(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    private DirectoryInfo CacheDir { get; }
    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DbSet<CachedGitSource> CachedGitSources { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
