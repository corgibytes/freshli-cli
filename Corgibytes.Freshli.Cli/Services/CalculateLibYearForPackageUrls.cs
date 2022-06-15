using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public class CalculateLibYearForPackageUrls
{
    private readonly List<IDependencyManagerRepository> _repositories;

    public CalculateLibYearForPackageUrls(List<IDependencyManagerRepository> repositories)
    {
        _repositories = repositories;
    }

    public LibYear GivenTwoPackages(PackageURL packageUrlCurrentlyInstalled, PackageURL packageUrlLatestAvailable)
    {
        if (packageUrlCurrentlyInstalled.Type != packageUrlLatestAvailable.Type)
        {
            throw new ArgumentException("Package URLs provided have different package managers");
        }

        // Validate the type, to see if we support them.
        // Since we already know they are the same, we only have to verify one.
        var dependencyManager = SupportedDependencyManagers.FromString(packageUrlCurrentlyInstalled.Type);

        // We got to find the repository that can give us information about the packages
        var repository = _repositories.Find(i => i.Supports().Equals(dependencyManager));

        if (repository == null)
        {
            throw new ArgumentException($"Repository not found that supports given dependency manager '{dependencyManager.DependencyManager()}'");
        }

        var releaseDatePackageCurrentlyInstalled =
            repository.GetReleaseDate(packageUrlCurrentlyInstalled.Name, packageUrlCurrentlyInstalled.Version);
        var releaseDatePackageLatestAvailable =
            repository.GetReleaseDate(packageUrlLatestAvailable.Name, packageUrlLatestAvailable.Version);

        return LibYear.GivenReleaseDates(releaseDatePackageCurrentlyInstalled, releaseDatePackageLatestAvailable);
    }
}

