using System;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DependencyManagers;

public interface IDependencyManagerRepository
{
    DateTimeOffset GetReleaseDate(PackageURL packageUrl);

    PackageURL GetLatestVersion(PackageURL packageUrl);
}
