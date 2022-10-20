using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    private readonly IInvoke _invoke;

    public AgentReader(IInvoke invoke, string agentExecutable)
    {
        _invoke = invoke;
        AgentExecutablePath = agentExecutable;
    }

    public string AgentExecutablePath { get; }

    public List<CachedPackage> RetrieveReleaseHistory(PackageURL packageUrl, ICacheManager cacheManager)
    {
        var packages = cacheManager.GetCacheDb().RetrieveReleaseHistory(packageUrl);
        if (packages.Count > 0)
        {
            return packages;
        }

        packages = new List<CachedPackage>();
        string packageUrlsWithDate;
        try
        {
            packageUrlsWithDate = _invoke.Command(AgentExecutablePath,
                $"retrieve-release-history {packageUrl.FormatWithoutVersion()}", ".");
        }
        catch (IOException)
        {
            return packages;
        }

        foreach (var packageUrlAndDate in packageUrlsWithDate.TrimEnd('\n', '\r').Split("\n"))
        {
            var separated = packageUrlAndDate.Split("\t");
            var cachedPackageUrl =
                new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, separated[0], null, null);

            packages.Add(
                new CachedPackage
                {
                    PackageName = cachedPackageUrl.FormatWithoutVersion(),
                    PackageUrl = cachedPackageUrl,
                    ReleasedAt = DateTimeOffset.ParseExact(separated[1], "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null)
                }
            );
        }

        cacheManager.GetCacheDb().AddReleaseHistory(packages);

        return packages;
    }

    public List<string> DetectManifests(string projectPath)
    {
        var manifests = _invoke.Command(AgentExecutablePath, $"detect-manifests {projectPath}", ".");

        return manifests.IsEmpty() ? new List<string>() : manifests.TrimEnd('\n', '\r').Split("\n").ToList();
    }

    public string ProcessManifest(string manifestPath, DateTime asOfDateTime)
    {
        var billOfMaterialsPath =
            _invoke.Command(AgentExecutablePath, $"process-manifest {manifestPath} {asOfDateTime:o}", ".");

        return billOfMaterialsPath.TrimEnd('\n', '\n');
    }
}
