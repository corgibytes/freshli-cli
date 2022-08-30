using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality.Analysis;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public interface IListCommits
{
    public IEnumerable<GitCommit> ForRepository(IAnalysisLocation analysisLocation, string gitPath);
}
