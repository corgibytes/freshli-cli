using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    public CachedGitSource FindOneByHash(string hash);

    public void Save(CachedGitSource cachedGitSource);

    public CachedGitSource CloneOrPull(string url, string? branch);
}
