using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Resources;

namespace Corgibytes.Freshli.Cli.Repositories;

public class CachedGitSourceRepository : ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, string cacheDir)
    {
        using var db = new CacheContext(cacheDir);
        var entry = db.CachedGitSources.Find(hash);
        if (entry == null)
        {
            throw new CacheException(CliOutput.CachedGitSourceRepository_No_Repository_Found_In_Cache);
        }

        return entry;
    }
}
