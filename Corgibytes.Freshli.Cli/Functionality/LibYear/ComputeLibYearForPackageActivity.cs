using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Services;
using Microsoft.Extensions.DependencyInjection;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity
{
    public PackageURL Package { get; init; }
    public DateTimeOffset AsOfDate { get; init; }
    public string AgentExecutablePath { get; init; }

    public void Handle(IApplicationEventEngine eventClient)
    {
        var agentManager = eventClient.ServiceProvider.GetRequiredService<IAgentManager>();
        var agentReader = agentManager.GetReader(AgentExecutablePath);

        var calculator = eventClient.ServiceProvider.GetRequiredService<IPackageLibYearCalculator>();
        var packageLibYear = calculator.ComputeLibYear(agentReader, Package, AsOfDate);

        eventClient.Fire(new LibYearComputedForPackageEvent { PackageLibYear = packageLibYear });
    }
}
