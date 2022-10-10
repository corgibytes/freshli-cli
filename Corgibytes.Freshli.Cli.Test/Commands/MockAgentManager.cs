using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Test.Commands;

public class MockAgentManager : IAgentManager
{
    public IAgentReader GetReader(string agentExecutablePath) => new MockAgentReader(agentExecutablePath);

    private class MockAgentReader : IAgentReader
    {
        internal MockAgentReader(string agentExecutable) => AgentExecutablePath = agentExecutable;

        public string AgentExecutablePath { get; }

        public List<CachedPackage> RetrieveReleaseHistory(PackageURL packageUrl, ICacheManager cacheManager) => AgentExecutablePath switch
        {
            "/usr/local/bin/freshli-agent-csharp" => packageUrl.Name switch
            {
                "flyswatter" => new List<CachedPackage>
                {
                    new()
                    {
                        Id = 1,
                        PackageName = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0").FormatWithoutVersion(),
                        PackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
                        ReleasedAt = new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero)
                    },
                    new()
                    {
                        Id = 1,
                        PackageName = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.2.0").FormatWithoutVersion(),
                        PackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.2.0"),
                        ReleasedAt = new DateTimeOffset(2001, 3, 14, 0, 0, 0, TimeSpan.Zero)
                    },
                    new()
                    {
                        Id = 1,
                        PackageName = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.3.0").FormatWithoutVersion(),
                        PackageUrl = new PackageURL("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.3.0"),
                        ReleasedAt = new DateTimeOffset(2020, 8, 21, 0, 0, 0, TimeSpan.Zero)
                    }
                },
                _ => new List<CachedPackage>()
            },
            "/usr/local/bin/freshli-agent-javascript" => new List<CachedPackage>(),
            "/usr/local/agents/bin/freshli-agent-csharp" => new List<CachedPackage>(),
            "/home/freshli-user/bin/agents/freshli-agent-ruby" => packageUrl.Name switch
            {
                "no_release_date" => new List<CachedPackage>
                {
                    new()
                    {
                        Id = 1,
                        PackageName = new PackageURL("pkg:ruby/org.corgibytes.no_release_date/no_release_date@2.3.0").FormatWithoutVersion(),
                        PackageUrl = new PackageURL("pkg:ruby/org.corgibytes.no_release_date/no_release_date@2.3.0"),
                        ReleasedAt = new DateTimeOffset(1990, 1, 29, 0, 0, 0, TimeSpan.Zero)
                    }
                },
                _ => new List<CachedPackage>()
            },
            _ => new List<CachedPackage>()
        };

        public List<string> DetectManifests(string projectPath) => throw new NotImplementedException();

        public string ProcessManifest(string manifestPath, DateTime asOfDate) => "/path/to/bill-of-materials";
    }
}
