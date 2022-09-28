using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisStartedEvent : IApplicationEvent
{
    public Guid AnalysisId { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) =>
        eventClient.Dispatch(new CreateAnalysisApiActivity(AnalysisId));
}
