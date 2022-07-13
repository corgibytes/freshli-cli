using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public interface IAgentReader
{
    public List<Package> ListValidPackageUrls(string agentExecutable, PackageURL packageUrl);
}
