using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class AnalysisApiCreatedEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }
    public Guid ApiAnalysisId { get; init; }
    public string RepositoryUrl { get; init; } = null!;

    public async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        if (Directory.Exists(RepositoryUrl))
        {
            await eventClient.Dispatch(new VerifyGitRepositoryInLocalDirectoryActivity { AnalysisId = AnalysisId });
        }
        else
        {
            await eventClient.Dispatch(new CloneGitRepositoryActivity(AnalysisId));
        }
    }
}
