using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    private readonly ICacheDb _cacheDb;
    private readonly ICommandInvoker _commandInvoker;

    public AgentReader(ICacheManager cacheManager, ICommandInvoker commandInvoker, string agentExecutable)
    {
        _cacheDb = cacheManager.GetCacheDb();
        _commandInvoker = commandInvoker;
        AgentExecutablePath = agentExecutable;
    }

    public string AgentExecutablePath { get; }

    public List<Package> RetrieveReleaseHistory(PackageURL packageUrl)
    {
        var cachedPackages = _cacheDb.RetrieveCachedReleaseHistory(packageUrl);

        if (cachedPackages.Count > 0)
        {
            return cachedPackages.Select(cachedPackage => cachedPackage.ToPackage()).ToList();
        }

        var packages = new List<Package>();
        string packageUrlsWithDate;
        try
        {
            packageUrlsWithDate = _commandInvoker.Run(AgentExecutablePath,
                $"retrieve-release-history {packageUrl.FormatWithoutVersion()}", ".", 3);
        }
        catch
        {
            return packages;
        }

        foreach (var packageUrlAndDate in packageUrlsWithDate.TrimEnd('\n', '\r').Split("\n"))
        {
            var separated = packageUrlAndDate.Split("\t");

            packages.Add(
                new Package(
                    new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, separated[0], null, null),
                    DateTimeOffset.ParseExact(separated[1], "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null)
                )
            );
        }

        _cacheDb.StoreCachedReleaseHistory(packages.Select(package => new CachedPackage(package)).ToList());

        return packages;
    }

    public List<string> DetectManifests(string projectPath)
    {
        var manifests = _commandInvoker.Run(AgentExecutablePath, $"detect-manifests {projectPath}", ".");

        return manifests.IsEmpty() ? new List<string>() : manifests.TrimEnd('\n', '\r').Split("\n").ToList();
    }

    public string ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime)
    {
        var billOfMaterialsPath =
            _commandInvoker.Run(AgentExecutablePath, $"process-manifest {manifestPath} {asOfDateTime:o}", ".");

        return billOfMaterialsPath.TrimEnd('\n', '\n');
    }
}
