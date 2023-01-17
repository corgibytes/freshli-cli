using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Agent;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PackageUrl;
using Package = Corgibytes.Freshli.Cli.Functionality.Package;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    private readonly ICacheDb _cacheDb;
    private readonly Agent.Agent.AgentClient _client;

    public AgentReader(ICacheManager cacheManager, Agent.Agent.AgentClient client)
    {
        _cacheDb = cacheManager.GetCacheDb();
        _client = client;
    }

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

        var request = new Agent.Package { Purl = packageUrl.FormatWithoutVersion() };
        var response = _client.RetrieveReleaseHistory(request);
        await foreach (var responseItem in response.ResponseStream.ReadAllAsync())
        {
            var package = new Package(
                new PackageURL(
                    packageUrl.Type,
                    packageUrl.Namespace,
                    packageUrl.Name,
                    responseItem.Version,
                    packageUrl.Qualifiers,
                    packageUrl.Subpath
                ),
                responseItem.ReleasedAt.ToDateTimeOffset()
            );
            packages.Add(package);
            yield return package;
        }

        await _cacheDb.StoreCachedReleaseHistory(packages.Select(package => new CachedPackage(package)).ToList());
    }

    public async IAsyncEnumerable<string> DetectManifests(string projectPath)
    {
        var request = new ProjectLocation() { Path = Path.GetFullPath(projectPath) };
        await foreach (var responseItem in _client.DetectManifests(request).ResponseStream.ReadAllAsync())
        {
            yield return responseItem.Path;
        }
    }

    public async ValueTask<string> ProcessManifest(string manifestPath, DateTimeOffset asOfDateTime)
    {
        var request = new ProcessingRequest
        {
            Manifest = new ManifestLocation() { Path = Path.GetFullPath(manifestPath) },
            Moment = asOfDateTime.ToTimestamp()
        };
        var response = await _client.ProcessManifestAsync(request);
        return response.Path;
    }
}
