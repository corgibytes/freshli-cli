using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiPackageLibYearActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public PackageLibYear PackageLibYear { get; init; } = null!;
    public string AgentExecutablePath { get; init; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);

        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        resultsApi.CreatePackageLibYear(cachedAnalysis!.ApiAnalysisId!.Value, PackageLibYear.AsOfDateTime,
            PackageLibYear);

        eventClient.Fire(new ApiPackageLibYearCreatedEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            AgentExecutablePath = AgentExecutablePath,
            PackageLibYear = PackageLibYear
        });
    }
}
