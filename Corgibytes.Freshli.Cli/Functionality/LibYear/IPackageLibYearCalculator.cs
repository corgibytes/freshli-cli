using System;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public interface IPackageLibYearCalculator
{
    // TODO: Make this method return ValueTask<PackageLibYear>
    public PackageLibYear ComputeLibYear(IAgentReader agentReader, PackageURL packageUrl, DateTimeOffset asOfDateTime);
}
