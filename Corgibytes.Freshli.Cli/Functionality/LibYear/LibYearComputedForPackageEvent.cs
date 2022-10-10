using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public class LibYearComputedForPackageEvent : IApplicationEvent
{
    public PackageLibYear PackageLibYear { get; init; }

    public void Handle(IApplicationActivityEngine eventClient) => throw new System.NotImplementedException();
}
