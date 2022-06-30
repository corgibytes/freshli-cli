using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheContext : DbContext
{
    public static DirectoryInfo DefaultCacheDir =>
        new(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    public DirectoryInfo CacheDir { get; }

    public const string CacheDbName = "freshli.db";
    public string DbPath { get; }

    public DbSet<CachedProperty> CachedProperties { get; set; }
    public DbSet<CachedGitRepo> CachedGitRepos { get; set; }

    public CacheContext(DirectoryInfo cacheDir)
    {
        CacheDir = cacheDir;
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}

public class CacheContextFactory : IDesignTimeDbContextFactory<CacheContext>
{
    public CacheContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CacheContext>();
        var cacheDir = CacheContext.DefaultCacheDir;
        var dbPath = Path.Join(cacheDir.ToString(), CacheContext.CacheDbName);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new CacheContext(cacheDir);
    }
}
