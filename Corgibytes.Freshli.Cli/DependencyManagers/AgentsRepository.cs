using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Services;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class AgentsRepository : IDependencyManagerRepository
{
    private readonly IAgentsDetector _agentsDetector;
    private readonly IAgentReader _agentReader;

    public AgentsRepository(IAgentsDetector agentsDetector, IAgentReader agentReader)
    {
        _agentsDetector = agentsDetector;
        _agentReader = agentReader;
    }

    public DateTimeOffset GetReleaseDate(PackageURL packageUrl)
    {
        foreach (var agentExecutable in _agentsDetector.Detect())
        {
            var validPackageUrls = _agentReader.ListValidPackageUrls(agentExecutable, packageUrl);
            if (validPackageUrls.Count == 0)
            {
                continue;
            }

            return GetReleaseDateForList(validPackageUrls, packageUrl);
        }

        throw ReleaseDateNotFoundException.BecauseNoAgentReturnedAnyResults();
    }

    private static DateTimeOffset GetReleaseDateForList(List<Package> validPackages, PackageURL packageUrl)
    {
        foreach (var package in validPackages)
        {
            // String comparing is not the neatest way, but it's the most reliable way
            // Otherwise we have to override the Equals method in PackageURL which we can't as it's a third-party library.
            if (package.PackageUrl.ToString() == packageUrl.ToString())
            {
                return package.ReleasedAt;
            }
        }

        throw ReleaseDateNotFoundException.BecauseReturnedListDidNotContainReleaseDate();
    }

    public PackageURL GetLatestVersion(PackageURL packageUrl)
    {
        foreach (var agentExecutable in _agentsDetector.Detect())
        {
            var packages = _agentReader.ListValidPackageUrls(agentExecutable, packageUrl);
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
}

