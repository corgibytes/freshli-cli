using System;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class UpdateAnalysisStatusActivity : IApplicationActivity
{
    public UpdateAnalysisStatusActivity(Guid apiAnalysisId, string status)
    {
        ApiAnalysisId = apiAnalysisId;
        Status = status;
    }

    public Guid ApiAnalysisId { get; }
    public string Status { get; }

    public async ValueTask Handle(IApplicationEventEngine eventClient, CancellationToken cancellationToken)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        await resultsApi.UpdateAnalysis(ApiAnalysisId, Status);

        await eventClient.Fire(new AnalysisApiStatusUpdatedEvent(ApiAnalysisId, Status), cancellationToken);
    }
}
