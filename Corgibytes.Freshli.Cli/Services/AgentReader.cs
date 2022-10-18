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
    private readonly IInvoke _invoke;

    public AgentReader(IInvoke invoke, string agentExecutable)
    {
        _invoke = invoke;
        AgentExecutablePath = agentExecutable;
    }

    public string AgentExecutablePath { get; }

    public List<Package> RetrieveReleaseHistory(PackageURL packageUrl)
    {
        var packages = new List<Package>();
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
