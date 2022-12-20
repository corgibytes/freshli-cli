using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async IAsyncEnumerable<Package> RetrieveReleaseHistory(PackageURL packageUrl)
    {
        var cachedPackages = _cacheDb.RetrieveCachedReleaseHistory(packageUrl);

        var isUsingCache = false;
        await foreach (var cachedPackage in cachedPackages)
        {
            isUsingCache = true;
            yield return cachedPackage.ToPackage();
        }

        if (isUsingCache)
        {
            yield break;
        }

        var packages = new List<Package>();
        string packageUrlsWithDate;
        try
        {
            packageUrlsWithDate = await _commandInvoker.Run(AgentExecutablePath,
                $"retrieve-release-history {packageUrl.FormatWithoutVersion()}", ".", 3);
        }
        catch
        {
            yield break;
        }

        foreach (var packageUrlAndDate in packageUrlsWithDate.TrimEnd(System.Environment.NewLine.ToCharArray()).Split(System.Environment.NewLine))
        {
            var separated = packageUrlAndDate.Split("\t");

            var package = new Package(
                new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, separated[0], null, null),
                DateTimeOffset.ParseExact(separated[1], "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null)
            );

            yield return package;
            packages.Add(package);
        }

        await _cacheDb.StoreCachedReleaseHistory(packages.Select(package => new CachedPackage(package)).ToList());
    }

    public async IAsyncEnumerable<string> DetectManifests(string projectPath)
    {
        var rawManifests = await _commandInvoker.Run(AgentExecutablePath, $"detect-manifests {projectPath}", ".");

        var manifestList = rawManifests.IsEmpty() ? new List<string>() : rawManifests.TrimEnd(System.Environment.NewLine.ToCharArray()).Split(System.Environment.NewLine).ToList();
        foreach (var manifest in manifestList)
        {
            yield return manifest;
        }
    }

    public async ValueTask<string> ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime)
    {
        var billOfMaterialsPath =
            await _commandInvoker.Run(AgentExecutablePath, $"process-manifest {manifestPath} {asOfDateTime:o}", ".");

        return billOfMaterialsPath.TrimEnd(System.Environment.NewLine.ToCharArray());
    }
}
