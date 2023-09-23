using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public interface IAgentReader
{
    public string Name { get; }
    public IAsyncEnumerable<Package> RetrieveReleaseHistory(PackageURL packageUrl);
    public IAsyncEnumerable<string> DetectManifests(string projectPath);
    public ValueTask<string> ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime);
}
