using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    public readonly IAnalysisLocation AnalysisLocation;
    public readonly ICalculateLibYearFromFile CalculateLibYearFromFile;
    public readonly string PathToBoM;

    public ComputeLibYearActivity(ICalculateLibYearFromFile calculateLibYearFromFile, string pathToBoM, IAnalysisLocation analysisLocation)
    {
        CalculateLibYearFromFile = calculateLibYearFromFile;
        PathToBoM = pathToBoM;
        AnalysisLocation = analysisLocation;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var libYearPackages = CalculateLibYearFromFile.AsList(PathToBoM);
        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            AnalysisLocation = AnalysisLocation
        });
    }
}
