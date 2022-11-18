using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface ICachedGitSourceRepository
{
    public ValueTask<CachedGitSource> FindOneByRepositoryId(string repositoryId);
    public ValueTask Save(CachedGitSource cachedGitSource);
    public ValueTask<CachedGitSource> CloneOrPull(string url, string? branch);
}
