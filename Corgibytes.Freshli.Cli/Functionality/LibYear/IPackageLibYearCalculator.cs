using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.LibYear;

public interface IPackageLibYearCalculator
{
    public ValueTask<PackageLibYear> ComputeLibYear(IAgentReader agentReader, PackageURL packageUrl, DateTimeOffset asOfDateTime);
}
