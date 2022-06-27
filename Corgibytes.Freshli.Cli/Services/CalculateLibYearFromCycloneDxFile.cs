using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Functionality;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Services;

public class CalculateLibYearFromCycloneDxFile : ICalculateLibYearFromFile
{
    private readonly ReadCycloneDxFile _readFile;
    private readonly IDependencyManagerRepository _repository;

    public CalculateLibYearFromCycloneDxFile(
        ReadCycloneDxFile readFileService,
        IDependencyManagerRepository repository
    )
    {
        _readFile = readFileService;
        _repository = repository;
    }

    public IList<PackageLibYear> AsList(string filePath, int precision = 2)
    {
        var packageUrls = _readFile.AsPackageURLs(filePath);
        var libYearList = new List<PackageLibYear>();

        foreach (var packageUrlCurrentlyInstalled in packageUrls)
        {
            var latestVersion =
                _repository.GetLatestVersion(packageUrlCurrentlyInstalled.Name);
            var releaseDatePackageCurrentlyInstalled =
                _repository.GetReleaseDate(packageUrlCurrentlyInstalled.Name, packageUrlCurrentlyInstalled.Version);
            var releaseDatePackageLatestAvailable =
                _repository.GetReleaseDate(packageUrlCurrentlyInstalled.Name, latestVersion);

            libYearList.Add(new(
                packageUrlCurrentlyInstalled.Name,
                releaseDatePackageCurrentlyInstalled,
                packageUrlCurrentlyInstalled.Version,
                releaseDatePackageLatestAvailable,
                latestVersion,
                LibYear.GivenReleaseDates(releaseDatePackageCurrentlyInstalled, releaseDatePackageLatestAvailable).AsDecimalNumber(precision)
            ));
        }

        return libYearList;
    }

    public double TotalAsDecimalNumber(string filePath, int precision = 2)
    {
        var packageUrls = _readFile.AsPackageURLs(filePath);
        var libYear = 0.0;

        foreach (var packageUrlCurrentlyInstalled in packageUrls)
        {
            var latestVersion =
                _repository.GetLatestVersion(packageUrlCurrentlyInstalled.Name);
            var releaseDatePackageCurrentlyInstalled =
                _repository.GetReleaseDate(packageUrlCurrentlyInstalled.Name, packageUrlCurrentlyInstalled.Version);
            var releaseDatePackageLatestAvailable =
                _repository.GetReleaseDate(packageUrlCurrentlyInstalled.Name, latestVersion);

            libYear += LibYear.GivenReleaseDates(releaseDatePackageCurrentlyInstalled, releaseDatePackageLatestAvailable).AsDecimalNumber(precision);
        }

        return Math.Round(libYear, precision);
    }
}

