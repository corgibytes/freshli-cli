using System;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class NuGet : IDependencyManagerRepository
{
    public DateTimeOffset GetReleaseDate(string name, string version) => (name, version) switch
    {
        ("calculatron", "21.3") => new(new(2022, 6, 16)),
        ("calculatron", "14.6") => new(new(2019, 12, 31)),
        ("flyswatter", "1.1.0") => new(new(1990, 1, 29)),
        ("auto-cup-of-tea", "112.0") => new(new(2004, 11, 11)),
        ("auto-cup-of-tea", "256.0") => new(new(2011, 10, 26)),
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };

    public string GetLatestVersion(string name) => name switch
    {
        "calculatron" => "21.3",
        "flyswatter" => "1.1.0",
        "auto-cup-of-tea" => "256.0",
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };

    public SupportedDependencyManagers Supports() => SupportedDependencyManagers.NuGet();
}

