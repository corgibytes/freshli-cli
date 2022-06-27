using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public interface IDependencyManagerRepository
{
    DateTimeOffset GetReleaseDate(PackageURL packageUrl);

    string GetLatestVersion(string name);
}

