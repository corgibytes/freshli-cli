using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

// This is just a mock for now and it should not be used like this.
// Once we have an actual agent we can use, then we can start processing the data from it.
public class AgentReader : IAgentReader
{
    public List<Package> ListValidPackageUrls(string agentExecutable, PackageURL packageUrl)
    {
        var packages = new List<Package>();
        var packageUrlsWithDate = Invoke.Command(agentExecutable, "validating-package-urls", ".");


        foreach (var packageUrlAndDate in packageUrlsWithDate.Split("\n"))
        {
            var separated = packageUrlAndDate.Split(" ");

            packages.Add(
                new
                (new(separated[0])
                    , DateTimeOffset.ParseExact(separated[1], "yyyy'-'MM'-'dd'T'HH':'mm':'ssK", null)
                ));
        }
        return packages;
    }
}
