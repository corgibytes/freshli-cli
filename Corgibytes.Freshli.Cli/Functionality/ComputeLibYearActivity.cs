using System;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    public readonly Guid AnalysisId;
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string PathToBoM;

    public ComputeLibYearActivity(Guid analysisId, string pathToBoM, IAnalysisLocation analysisLocation)
    {
        AnalysisId = analysisId;
        PathToBoM = pathToBoM;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var calculateLibYearFromFile = eventClient.ServiceProvider.GetRequiredService<ICalculateLibYearFromFile>();
        var libYearPackages = calculateLibYearFromFile.AsList(PathToBoM);

        var cacheManager = eventClient.ServiceProvider.GetRequiredService<ICacheManager>();
        var cacheDb = cacheManager.GetCacheDb();
        foreach (var libYearPackage in libYearPackages)
        {
            cacheDb.AddLibYear(new CachedLibYear()
            {
                PackageName = libYearPackage.PackageUrl?.Name,
                CurrentVersion = libYearPackage.CurrentVersion?.ToString(),
                ReleaseDateCurrentVersion = libYearPackage.ReleaseDateCurrentVersion,
                LatestVersion = libYearPackage.LatestVersion?.ToString(),
                ReleaseDateLatestVersion = libYearPackage.ReleaseDateLatestVersion,
                LibYear = libYearPackage.LibYear,
                HistoryIntervalStopId = AnalysisLocation.HistoryIntervalStopId
            });
        }

        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            AnalysisLocation = AnalysisLocation
        });
    }
}
