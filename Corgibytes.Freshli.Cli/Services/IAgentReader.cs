using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public interface IAgentReader
{
    public string AgentExecutablePath { get; }
    public List<CachedPackage> RetrieveReleaseHistory(PackageURL packageUrl);
    public List<string> DetectManifests(string projectPath);
    public string ProcessManifest(string manifestPath, DateTime asOfDate);
}
