using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class ListCommits : IListCommits
{
    public IEnumerable<GitCommit> ForRepository(string repositoryId, string gitPath) =>
        throw new NotImplementedException();
}
