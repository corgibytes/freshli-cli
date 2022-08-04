using System;
using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public class AgentReader : IAgentReader
{
    public List<Package> RetrieveReleaseHistory(string agentExecutable, PackageURL packageUrl)
    {
        var packages = new List<Package>();
        string packageUrlsWithDate;
        try
        {
            packageUrlsWithDate = Invoke.Command(agentExecutable,
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
                new
                (new(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, separated[0], null, null)
                    , DateTimeOffset.ParseExact(separated[1], "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null)
                ));
        }

        return packages;
    }
}
