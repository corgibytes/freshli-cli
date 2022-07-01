namespace Corgibytes.Freshli.Cli.Functionality;

public class GitCommitIdentifier
{
    private string CommitId { get; }

    public GitCommitIdentifier(string commitId)
    {
        if (commitId.Length > 40 || commitId.Length < 7)
        {
            throw new GitException($"Git commit id ({commitId}) not valid. It should be between 7 and 40 characters long. Given string is {commitId.Length} characters long");
        }

        CommitId = commitId;
    }
}

