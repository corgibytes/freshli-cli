using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
    public static string DefaultCacheDir =>
        Path.Combine(Environment.GetEnvironmentVariable("HOME")!, ".freshli");

    private DirectoryInfo CacheDir { get; }
    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties => Set<CachedProperty>();
    public DbSet<CachedGitSource> CachedGitSources => Set<CachedGitSource>();
    public DbSet<CachedAnalysis> CachedAnalyses => Set<CachedAnalysis>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}

// ReSharper disable once UnusedType.Global
public class CacheContextFactory : IDesignTimeDbContextFactory<CacheContext>
{
    public CacheContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CacheContext>();
        var cacheDir = new DirectoryInfo(CacheContext.DefaultCacheDir);
        var dbPath = Path.Join(cacheDir.ToString(), CacheContext.CacheDbName);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new(cacheDir);
    }
}
