using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Corgibytes.Freshli.Cli.DataModel;

// This class enables the use of the `dotnet ef` commands to manage migrations.
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
