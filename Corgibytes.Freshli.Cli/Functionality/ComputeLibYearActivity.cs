using Corgibytes.Freshli.Cli.Functionality.Analysis;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corgibytes.Freshli.Cli.Functionality;

public class ComputeLibYearActivity : IApplicationActivity
{
    public readonly IHistoryStopData HistoryStopData;
    public readonly string PathToBoM;

    public ComputeLibYearActivity(string pathToBoM, IHistoryStopData historyStopData)
    {
        PathToBoM = pathToBoM;
        HistoryStopData = historyStopData;
    }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var calculateLibYearFromFile = eventClient.ServiceProvider.GetRequiredService<ICalculateLibYearFromFile>();
        var libYearPackages = calculateLibYearFromFile.AsList(PathToBoM);
        eventClient.Fire(new LibYearComputedEvent
        {
            LibYearPackages = libYearPackages,
            HistoryStopData = HistoryStopData
        });
    }
}
