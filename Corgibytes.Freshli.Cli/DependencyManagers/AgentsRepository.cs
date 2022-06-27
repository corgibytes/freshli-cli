using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public class AgentsRepository : IDependencyManagerRepository
{
    public DateTimeOffset GetReleaseDate(PackageURL packageUrl) => (packageUrl.Name + " " + packageUrl.Version) switch
    {
        ("calculatron 1.3") => new(2022, 6, 16, 0, 0, 0, TimeSpan.Zero),
        ("calculatron 14.6") => new(2019, 12, 31, 0, 0, 0, TimeSpan.Zero),
        ("flyswatter 1.1.0") => new(1990, 1, 29, 0, 0, 0, TimeSpan.Zero),
        ("auto-cup-of-tea 112.0") => new(2004, 11, 11, 0, 0, 0, TimeSpan.Zero),
        ("auto-cup-of-tea 256.0") => new(2011, 10, 26, 0, 0, 0, TimeSpan.Zero),
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };

    public PackageURL GetLatestVersion(PackageURL packageUrl) => packageUrl.Name switch
    {
        "calculatron" => new("pkg:nuget/org.corgibytes.calculatron/calculatron@21.3"),
        "flyswatter" => new("pkg:nuget/org.corgibytes.flyswatter/flyswatter@1.1.0"),
        "auto-cup-of-tea" => new("pkg:nuget/org.corgibytes.auto-cup-of-tea/auto-cup-of-tea@256.0"),
        _ => throw new ArgumentException("Mock date could not be returned. Forgot to add it?")
    };
}

