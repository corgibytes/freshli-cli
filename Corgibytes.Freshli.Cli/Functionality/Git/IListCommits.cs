using System.Collections.Generic;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IListCommits
{
    public IEnumerable<GitCommit> ForRepository(string repositoryId, string cacheDir, string gitPath);
}
