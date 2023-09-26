using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public DbSet<CachedManifest> CachedManifests => Set<CachedManifest>();
    public DbSet<CachedPackageLibYear> CachedPackageLibYears => Set<CachedPackageLibYear>();
    public DbSet<CachedPackage> CachedPackages => Set<CachedPackage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlite($"Data Source={DbPath}");

    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    // Based on: https://stackoverflow.com/a/63421380/243215
    private void AddTimestamps()
    {
        var entities = ChangeTracker.Entries().Where(entry =>
            entry is
            {
                Entity: TimeStampedEntity,
                State: EntityState.Added or EntityState.Modified
            }
        );

        foreach (var entity in entities)
        {
            var now = DateTime.UtcNow;

            var timeStampedEntity = (TimeStampedEntity)entity.Entity;
            if (entity.State == EntityState.Added)
            {
                timeStampedEntity.CreatedAt = now;
            }
            timeStampedEntity.UpdatedAt = now;
        }
    }
}
