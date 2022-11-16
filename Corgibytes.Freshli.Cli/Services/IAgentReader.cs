using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public interface IAgentReader
{
    public string AgentExecutablePath { get; }
    // TODO: Make this method return ValueTask or an async-friendly enumerable
    public List<Package> RetrieveReleaseHistory(PackageURL packageUrl);
    // TODO: Make this method return ValueTask or an async-friendly enumerable
    public List<string> DetectManifests(string projectPath);
    // TODO: Make this method return ValueTask<string>
    public string ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime);
}
