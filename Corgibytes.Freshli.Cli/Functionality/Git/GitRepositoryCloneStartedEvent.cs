using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryCloneStartedEvent : IApplicationEvent
{
    public required Guid AnalysisId { get; init; }

    public async ValueTask Handle(IApplicationActivityEngine eventClient)
    {
        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportGitOperationStarted(GitOperation.CreateNewClone);
    }
}
