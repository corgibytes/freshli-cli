using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;
using ServiceStack;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    public AgentReader(string agentExecutable) => AgentExecutablePath = agentExecutable;

    public string AgentExecutablePath { get; }

    public List<Package> RetrieveReleaseHistory(PackageURL packageUrl)
    {
        var packages = new List<Package>();
        string packageUrlsWithDate;
        try
        {
            packageUrlsWithDate = Invoke.Command(AgentExecutablePath,
                $"retrieve-release-history {packageUrl.FormatWithoutVersion()}", ".");
        }
        catch (IOException)
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

        return packages;
    }

    public List<string> DetectManifests(string projectPath)
    {
        var manifests = Invoke.Command(AgentExecutablePath, $"detect-manifests {projectPath}", ".");

        return manifests.IsEmpty() ? new List<string>() :
            manifests.TrimEnd('\n', '\r').Split("\n").ToList();
    }

    public string ProcessManifest(string manifestPath, DateTime asOfDate)
    {
        var billOfMaterialsPath =
            Invoke.Command(AgentExecutablePath, $"process-manifest {manifestPath} {asOfDate:o}", ".");

        return billOfMaterialsPath.TrimEnd('\n', '\n');
    }
}
