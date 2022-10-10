using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Corgibytes.Freshli.Cli.DataModel;

public class CacheContext : DbContext
{
    public const string CacheDbName = "freshli.db";

    public CacheContext(string cacheDir)
    {
        CacheDir = new DirectoryInfo(cacheDir);
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    // ReSharper disable once UnusedMember.Global
    private DirectoryInfo CacheDir { get; }
    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties => Set<CachedProperty>();
    public DbSet<CachedGitSource> CachedGitSources => Set<CachedGitSource>();
    public DbSet<CachedAnalysis> CachedAnalyses => Set<CachedAnalysis>();
    public DbSet<CachedHistoryIntervalStop> CachedHistoryIntervalStops => Set<CachedHistoryIntervalStop>();
    public DbSet<CachedPackage> CachedPackages => Set<CachedPackage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}

// ReSharper disable once UnusedType.Global
public class CacheContextFactory : IDesignTimeDbContextFactory<CacheContext>
{
    public CacheContext CreateDbContext(string[] args)
    {
        var configuration = new Configuration(new Environment());
        var optionsBuilder = new DbContextOptionsBuilder<CacheContext>();
        var dbPath = Path.Join(configuration.CacheDir, CacheContext.CacheDbName);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new CacheContext(configuration.CacheDir);
    }
}
