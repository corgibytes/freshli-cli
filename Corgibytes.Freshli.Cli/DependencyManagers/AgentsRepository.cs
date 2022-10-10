using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using Newtonsoft.Json;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class AgentsRepository : IDependencyManagerRepository
{
    [JsonProperty] private readonly IAgentManager _agentManager;
    [JsonProperty] private readonly ICacheManager _cacheManager;
    [JsonProperty] private readonly IAgentsDetector _agentsDetector;

    public AgentsRepository(IAgentsDetector agentsDetector, IAgentManager agentManager, ICacheManager cacheManager)
    {
        _agentsDetector = agentsDetector;
        _agentManager = agentManager;
        _cacheManager = cacheManager;
    }

    public DateTimeOffset GetReleaseDate(PackageURL packageUrl)
    {
        foreach (var agentExecutable in _agentsDetector.Detect())
        {
            var releaseHistory = _agentManager.GetReader(agentExecutable).RetrieveReleaseHistory(packageUrl, _cacheManager);
            if (releaseHistory.Count == 0)
            {
                continue;
            }

            return GetReleaseDateForList(releaseHistory, packageUrl);
        }

        throw ReleaseDateNotFoundException.BecauseNoAgentReturnedAnyResults();
    }

    public PackageURL GetLatestVersion(PackageURL packageUrl)
    {
        foreach (var agentExecutable in _agentsDetector.Detect())
        {
            var packages = _agentManager.GetReader(agentExecutable).RetrieveReleaseHistory(packageUrl, _cacheManager);
            if (packages.Count == 0)
            {
                continue;
            }

            var latestPackage = packages.MaxBy(package => package.ReleasedAt);
            if (latestPackage != null)
            {
                return latestPackage.PackageUrl;
            }
        }

        throw LatestVersionNotFoundException.BecauseLatestCouldNotBeFoundInList();
    }

    private static DateTimeOffset GetReleaseDateForList(List<CachedPackage> releaseHistory, PackageURL packageUrl)
    {
        foreach (var package in releaseHistory.Where(package => package.PackageUrl.PackageUrlEquals(packageUrl)))
        {
            return package.ReleasedAt;
        }

        throw ReleaseDateNotFoundException.BecauseReturnedListDidNotContainReleaseDate();
    }
}
