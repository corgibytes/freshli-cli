using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheContext : DbContext
{
    #nullable enable
    public static readonly DirectoryInfo DefaultCacheDir =
        new DirectoryInfo(Environment.GetEnvironmentVariable("HOME") + "/.freshli");

    private static DirectoryInfo? s_cacheDir;

    public static DirectoryInfo CacheDir
    {
        get => s_cacheDir ?? DefaultCacheDir;
        set => s_cacheDir = value;
    }
    #nullable restore

    public static readonly string CacheDbName = "freshli.db";
    private string DbPath { get; }

    public DbSet<CachedProperty> CachedProperties { get; set; }

    public CacheContext()
    {
        DbPath = Path.Join(CacheDir.ToString(), CacheDbName);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}
