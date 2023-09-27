using System;
using System.Threading;
using Corgibytes.Freshli.Cli.Functionality.LibYear;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.Agents;

public interface IAgentManager
{
    public IAgentReader GetReader(string agentExecutablePath, CancellationToken cancellationToken = default);
    public IPackageLibYearCalculator GetLibYearCalculator(IAgentReader reader, PackageURL packageUrl,
        DateTimeOffset asOfDateTime);
}
