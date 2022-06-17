using System;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public class CalculateLibYearFromCycloneDxFile : ICalculateLibYearFromFile
{
    private readonly ReadCycloneDxFile _readFile;
    private readonly IEnumerable<IDependencyManagerRepository> _repositories;
    private readonly CalculateLibYearForPackageUrls _calculateLibYearForPackageUrls;

    public CalculateLibYearFromCycloneDxFile(
        ReadCycloneDxFile readFileService,
        IEnumerable<IDependencyManagerRepository> repositories,
        CalculateLibYearForPackageUrls calculateLibYearForPackageUrls
    )
    {
        _readFile = readFileService;
        _repositories = repositories;
        _calculateLibYearForPackageUrls = calculateLibYearForPackageUrls;
    }

    public double AsDecimalNumber(string filePath, int precision = 2)
    {
        var packageUrls = _readFile.AsPackageURLs(filePath);
        var libYear = 0.0;

        foreach (var packageUrlCurrentlyInstalled in packageUrls)
        {
            var dependencyManager = SupportedDependencyManagers.FromString(packageUrlCurrentlyInstalled.Type);
            var repository = _repositories.ToList().Find(i => i.Supports().Equals(dependencyManager));

            if (repository == null)
            {
                throw new ArgumentException($"Repository not found that supports given dependency manager '{dependencyManager.DependencyManager()}'");
            }

            var packageUrlLatestVersion = new PackageURL(
                packageUrlCurrentlyInstalled.Type,
                packageUrlCurrentlyInstalled.Namespace,
                packageUrlCurrentlyInstalled.Name,
                repository.GetLatestVersion(packageUrlCurrentlyInstalled.Name),
                packageUrlCurrentlyInstalled.Qualifiers,
                packageUrlCurrentlyInstalled.Subpath
            );

            libYear += _calculateLibYearForPackageUrls.GivenTwoPackages(packageUrlCurrentlyInstalled, packageUrlLatestVersion).AsDecimalNumber(precision);
        }

        return Math.Round(libYear, precision);
    }
}

