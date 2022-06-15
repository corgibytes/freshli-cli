using System;
using Corgibytes.Freshli.Cli.DependencyManagers;

namespace Corgibytes.Freshli.Cli.Test.DependencyManagers;

public class MockNuGetDependencyManagerRepository: IDependencyManagerRepository
{
    public DateTime GetReleaseDate(string name, string version)
    {
        if (name == "Newtonsoft.Json" && version == "3.22.2021")
        {
            return new DateTime(2021, 3, 22);
        }

        if (name == "Newtonsoft.Json" && version == "8.3.2014")
        {
            return new DateTime(2014, 8, 3);
        }

        throw new ArgumentException("Mock date could not be returned. Forgot to add it?");
    }

    public SupportedDependencyManagers Supports() => SupportedDependencyManagers.NuGet();
}

