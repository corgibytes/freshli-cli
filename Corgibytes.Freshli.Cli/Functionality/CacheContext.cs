using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        new(System.Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    private DirectoryInfo CacheDir { get; }
    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties => Set<CachedProperty>();
    public DbSet<CachedGitRepo> CachedGitRepos => Set<CachedGitRepo>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}

// ReSharper disable once UnusedType.Global
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
