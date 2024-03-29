using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.History;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class GitRepositoryClonedEvent : ApplicationEventBase
{
    public Guid AnalysisId { get; init; }

    public HistoryStopData HistoryStopData { get; init; } = null!;

    public override async ValueTask Handle(IApplicationActivityEngine eventClient, CancellationToken cancellationToken)
    {
        var progressReporter = eventClient.ServiceProvider.GetRequiredService<IAnalyzeProgressReporter>();
        progressReporter.ReportGitOperationFinished(GitOperation.CreateNewClone);
        await eventClient.Dispatch(new ComputeHistoryActivity(AnalysisId, HistoryStopData), cancellationToken);
    }
}
