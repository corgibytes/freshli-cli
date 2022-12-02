using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisStartedEvent : ApplicationEventBase
{
    public Guid AnalysisId { get; init; }

    public override async ValueTask Handle(IApplicationActivityEngine eventClient) =>
        await eventClient.Dispatch(new CreateAnalysisApiActivity(AnalysisId));
}
