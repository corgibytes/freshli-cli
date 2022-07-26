using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

// This is just a mock for now and it should not be used like this.
// Once we have an actual agent we can use, then we can start processing the data from it.
public class AgentReader : IAgentReader
{
    public List<Package> ListValidPackageUrls(string agentExecutable, PackageURL packageUrl)
    {
        var package = new List<Package>();

        var processInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            FileName = agentExecutable,
            Arguments = "validating-package-urls",
            RedirectStandardOutput = true
        };
        var process = Process.Start(processInfo); // Start that process.


        while (process != null && !process.StandardOutput.EndOfStream)
        {
            var fileData = process.StandardOutput.ReadLine()?.Split(" ");
            if (fileData != null)
            {
                package.Add(new(new(fileData[0]), DateTimeOffset.Parse(fileData[1])));
            }
        }

        process?.WaitForExit();


        return package;
    }
}
