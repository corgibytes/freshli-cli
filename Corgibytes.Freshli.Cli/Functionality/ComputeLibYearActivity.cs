using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly string PathToBoM;

    public ComputeLibYearActivity(string pathToBoM, IAnalysisLocation analysisLocation)
    {
        PathToBoM = pathToBoM;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var calculateLibYearFromFile = eventClient.ServiceProvider.GetRequiredService<ICalculateLibYearFromFile>();
        var libYearPackages = calculateLibYearFromFile.AsList(PathToBoM);
        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            AnalysisLocation = AnalysisLocation
        });
    }
}
