using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IListCommits
{
    public IEnumerable<GitCommit> ForRepository(string repositoryId, string gitPath);
}
