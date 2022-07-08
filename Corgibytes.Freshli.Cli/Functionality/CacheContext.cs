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
        new(System.Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    private DirectoryInfo CacheDir { get; }
    private string DbPath { get; }

    // ReSharper disable once UnusedMember.Global
    public DbSet<CachedProperty> CachedProperties { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DbSet<CachedGitRepo> CachedGitRepos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
