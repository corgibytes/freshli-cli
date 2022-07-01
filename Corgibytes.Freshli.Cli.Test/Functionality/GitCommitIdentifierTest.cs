using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class GitCommitIdentifierTest : FreshliTest
{
    public GitCommitIdentifierTest(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [MethodData(nameof(LengthVerifications))]
    public void Verify_length_requirements(string commitId, string expectedErrorMessage)
    {
        var caughtException = Assert.Throws<GitException>(() => new GitCommitIdentifier(commitId));
        Assert.Equal(expectedErrorMessage, caughtException.Message);
    }

    private static TheoryData<string, string> LengthVerifications()
    {
        return new()
        {
            {"bb97606e413706874d93185f58fe452448ac6680toomany", "Git commit id (bb97606e413706874d93185f58fe452448ac6680toomany) not valid. It should be between 7 and 40 characters long. Given string is 47 characters long"},
            {"bb97", "Git commit id (bb97) not valid. It should be between 7 and 40 characters long. Given string is 4 characters long"}
        };
    }
}

