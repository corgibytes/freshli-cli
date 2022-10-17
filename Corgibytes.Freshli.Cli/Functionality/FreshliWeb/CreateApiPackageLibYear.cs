using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class CreateApiPackageLibYearActivity : IApplicationActivity
{
    public Guid AnalysisId { get; init; }
    public int HistoryStopPointId { get; init; }
    public int PackageLibYearId { get; init; }
    public string AgentExecutablePath { get; init; } = null!;

    public void Handle(IApplicationEventEngine eventClient)
    {
        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();

        var cachedAnalysis = cacheDb.RetrieveAnalysis(AnalysisId);
        var packageLibYear = cacheDb.RetrieveLibYear(PackageLibYearId);

        var resultsApi = eventClient.ServiceProvider.GetRequiredService<IResultsApi>();
        resultsApi.CreatePackageLibYear(cachedAnalysis!.ApiAnalysisId!.Value, packageLibYear.AsOfDateTime,
            packageLibYear);

        eventClient.Fire(new ApiPackageLibYearCreatedEvent
        {
            AnalysisId = AnalysisId,
            HistoryStopPointId = HistoryStopPointId,
            PackageLibYearId = PackageLibYearId,
            AgentExecutablePath = AgentExecutablePath
        });
    }
}
