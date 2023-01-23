using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

public class CacheContext : DbContext
{
    public const string CacheDbName = "freshli.db";

    public CacheContext(string cacheDir)
    {
        DbPath = Path.Join(cacheDir, CacheDbName);
    }

    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties => Set<CachedProperty>();
    public DbSet<CachedGitSource> CachedGitSources => Set<CachedGitSource>();
    public DbSet<CachedAnalysis> CachedAnalyses => Set<CachedAnalysis>();
    public DbSet<CachedHistoryStopPoint> CachedHistoryStopPoints => Set<CachedHistoryStopPoint>();
    public DbSet<CachedPackageLibYear> CachedPackageLibYears => Set<CachedPackageLibYear>();
    public DbSet<CachedPackage> CachedPackages => Set<CachedPackage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
