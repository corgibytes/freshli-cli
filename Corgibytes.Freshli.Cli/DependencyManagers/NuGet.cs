using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class NuGet : IDependencyManagerRepository
{
    public DateTime GetReleaseDate(string name, string version)
    {
        if (name == "calculatron" && version == "21.3")
        {
            return new(2022, 6, 16);
        }

        if (name == "calculatron" && version == "14.6")
        {
            return new(2019, 12, 31);
        }

        if (name == "flyswatter" && version == "1.1.0")
        {
            return new(1990, 1, 29);
        }

        if (name == "auto-cup-of-tea" && version == "112.0")
        {
            return new(2006, 11, 11);
        }

        if (name == "auto-cup-of-tea" && version == "256.0")
        {
            return new(2011, 10, 26);
        }

        throw new ArgumentException("Mock date could not be returned. Forgot to add it?");
    }

    public string GetLatestVersion(string name) => name switch
    {
        "calculatron" => "21.3",
        "flyswatter" => "1.1.0",
        "auto-cup-of-tea" => "256.0",
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };

    public SupportedDependencyManagers Supports() => SupportedDependencyManagers.NuGet();
}

