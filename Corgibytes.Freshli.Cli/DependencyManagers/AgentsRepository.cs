using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class AgentsRepository : IDependencyManagerRepository
{
    public DateTimeOffset GetReleaseDate(PackageURL packageUrl) => (packageUrl.Name + " " + packageUrl.Version) switch
    {
        ("calculatron 1.3") => new(new(2022, 6, 16)),
        ("calculatron 14.6") => new(new(2019, 12, 31)),
        ("flyswatter 1.1.0") => new(new(1990, 1, 29)),
        ("auto-cup-of-tea 112.0") => new(new(2004, 11, 11)),
        ("auto-cup-of-tea 256.0") => new(new(2011, 10, 26)),
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };

    public string GetLatestVersion(PackageURL packageUrl) => packageUrl.Name switch
    {
        "calculatron" => "21.3",
        "flyswatter" => "1.1.0",
        "auto-cup-of-tea" => "256.0",
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };
}

