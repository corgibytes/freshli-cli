using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class UpdateAnalysisStatusActivity : IApplicationActivity
{
    public UpdateAnalysisStatusActivity(Guid apiAnalysisId, string status)
    {
        ApiAnalysisId = apiAnalysisId;
        Status = status;
    }

    public Guid ApiAnalysisId { get; }
    public string Status { get; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();

        resultsApi.UpdateAnalysis(ApiAnalysisId, Status);

        eventClient.Fire(new AnalysisApiStatusUpdatedEvent(ApiAnalysisId, Status));
    }
}
