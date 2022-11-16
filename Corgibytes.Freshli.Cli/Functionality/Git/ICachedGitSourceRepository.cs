using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    // TODO: Make this method return ValueTask<CachedGitStore>
    public CachedGitSource FindOneByRepositoryId(string repositoryId);

    // TODO: Make this method return ValueTask
    public void Save(CachedGitSource cachedGitSource);

    // TODO: Make this method return ValueTask<CachedGitStore>
    public CachedGitSource CloneOrPull(string url, string? branch);
}
