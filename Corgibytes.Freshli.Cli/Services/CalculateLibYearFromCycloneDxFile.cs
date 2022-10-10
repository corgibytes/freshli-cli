using System;
using System.Collections.Generic;
using Corgibytes.Freshli.Cli.DependencyManagers;
using Corgibytes.Freshli.Cli.Functionality;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Services;

public class CalculateLibYearFromCycloneDxFile : ICalculateLibYearFromFile
{
    [JsonProperty] private readonly ReadCycloneDxFile _readFile;

    [JsonProperty] private readonly IDependencyManagerRepository _repository;

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
        var packageUrls = _readFile.AsPackageUrls(filePath);
        var libYearList = new List<PackageLibYear>();

        foreach (var currentlyInstalled in packageUrls)
        {
            try
            {
                var latestVersion =
                    _repository.GetLatestVersion(currentlyInstalled);
                var releaseDatePackageCurrentlyInstalled =
                    _repository.GetReleaseDate(currentlyInstalled);
                var releaseDatePackageLatestAvailable =
                    _repository.GetReleaseDate(latestVersion);

                libYearList.Add(new PackageLibYear(
                    releaseDatePackageCurrentlyInstalled,
                    currentlyInstalled,
                    releaseDatePackageLatestAvailable,
                    latestVersion,
                    LibYear2.GivenReleaseDates(releaseDatePackageCurrentlyInstalled, releaseDatePackageLatestAvailable)
                        .AsDecimalNumber(precision),
                    DateTimeOffset.MinValue // todo: this should be the date that is being analyzed
                ));
            }
            catch (Exception exception)
            {
                libYearList.Add(new PackageLibYear(
                    currentlyInstalled,
                    exception.Message
                ));
            }
        }

        return libYearList;
    }
}
