using System;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Analysis;

public class AnalysisLocationTest
{
    [Fact]
    public void PathWithoutCommitId()
    {
        var cacheDirectory = "/path/to/cache/dir";
        var repositoryId = Guid.NewGuid().ToString();
        var location = new AnalysisLocation(cacheDirectory, repositoryId);

        var expectedPath = Path.Combine(cacheDirectory, "repositories", repositoryId);
        Assert.Equal(expectedPath, location.Path);
    }

    [Fact]
    public void PathWithCommitId()
    {
        var cacheDirectory = "/path/to/cache/dir";
        var repositoryId = Guid.NewGuid().ToString();
        var commitId = Guid.NewGuid().ToString();
        var location = new AnalysisLocation(cacheDirectory, repositoryId, commitId);

        var expectedPath = Path.Combine(cacheDirectory, "histories", repositoryId, commitId);
        Assert.Equal(expectedPath, location.Path);
    }
}
