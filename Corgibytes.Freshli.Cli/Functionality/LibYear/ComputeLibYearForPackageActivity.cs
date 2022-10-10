using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class ComputeLibYearForPackageActivity : IApplicationActivity
{
    public PackageURL Package { get; init; }
    public DateTimeOffset AsOfDate { get; init; }
    public string AgentExecutablePath { get; init; }

    public void Handle(IApplicationEventEngine eventClient) => throw new System.NotImplementedException();
}
