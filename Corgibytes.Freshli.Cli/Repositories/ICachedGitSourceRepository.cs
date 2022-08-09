using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Repositories;

public interface ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash, string cacheDir);
}
