using System;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public interface IPackageLibYearCalculator
{
    public double ComputeLibYear(IAgentReader agentReader, PackageURL packageUrl, DateTimeOffset asOfDate);
}
