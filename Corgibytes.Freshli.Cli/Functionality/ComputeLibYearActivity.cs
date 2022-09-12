using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    [JsonProperty] private readonly IAnalysisLocation _analysisLocation;
    [JsonProperty] private readonly ICalculateLibYearFromFile _calculateLibYearFromFile;

    [JsonProperty] private readonly string _pathToBoM;

    public ComputeLibYearActivity(ICalculateLibYearFromFile calculateLibYearFromFile, string pathToBoM, IAnalysisLocation analysisLocation)
    {
        _calculateLibYearFromFile = calculateLibYearFromFile;
        _pathToBoM = pathToBoM;
        _analysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var libYearPackages = _calculateLibYearFromFile.AsList(_pathToBoM);
        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            AnalysisLocation = _analysisLocation
        });
    }
}
