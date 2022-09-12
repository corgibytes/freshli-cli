using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    public readonly IAnalysisLocation AnalysisLocation;
    [JsonProperty] private readonly ICalculateLibYearFromFile _calculateLibYearFromFile;
    public readonly string PathToBoM;

    public ComputeLibYearActivity(ICalculateLibYearFromFile calculateLibYearFromFile, string pathToBoM,
        IAnalysisLocation analysisLocation)
    {
        _calculateLibYearFromFile = calculateLibYearFromFile;
        PathToBoM = pathToBoM;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var libYearPackages = _calculateLibYearFromFile.AsList(PathToBoM);
        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            AnalysisLocation = AnalysisLocation
        });
    }
}
