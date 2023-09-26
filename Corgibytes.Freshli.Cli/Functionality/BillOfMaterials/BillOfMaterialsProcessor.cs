using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using CycloneDX.Models;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsProcessor : IBillOfMaterialsProcessor
{
    private readonly ICacheManager _cacheManager;

    public BillOfMaterialsProcessor(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task AddLibYearMetadataDataToBom(CachedManifest manifest, string agentExecutablePath, string pathToBom,
        CancellationToken cancellationToken)
    {
        await using var bomReadStream = File.OpenRead(pathToBom);
        var bom = await CycloneDX.Json.Serializer.DeserializeAsync(bomReadStream);
        bomReadStream.Close();

        if (bom == null)
        {
            throw new Exception("Unable to open bom");
        }

        AddFreshliMetadataProperties(bom, manifest);
        await AddFreshliComponentProperties(bom, manifest);

        await using var bomWriteStream = File.Open(pathToBom, FileMode.Truncate);
        await CycloneDX.Json.Serializer.SerializeAsync(bom, bomWriteStream);
        await bomWriteStream.FlushAsync(cancellationToken);
        bomWriteStream.Close();
    }

    private static void AddFreshliMetadataProperties(Bom bom, CachedManifest manifest) =>
        bom.AddMetadataProperties(new List<Property>()
        {
            new()
            {
                Name = "freshli:analysis:id",
                Value = manifest.HistoryStopPoint.CachedAnalysis.Id.ToString()
            },
            new()
            {
                Name = "freshli:analysis:creation-date",
                Value = manifest.HistoryStopPoint.CachedAnalysis.CreatedAt!.Value.ToString("O")
            },
            new()
            {
                Name = "freshli:analysis:data-point",
                Value = manifest.HistoryStopPoint.AsOfDateTime.ToString("O")
            },
            new()
            {
                Name = "freshli:source:url",
                Value = manifest.HistoryStopPoint.Repository.Url
            },
            new()
            {
                Name = "freshli:source:branch",
                Value = manifest.HistoryStopPoint.Repository.Branch!
            },
            new()
            {
                Name = "freshli:source:clone-path",
                Value = manifest.HistoryStopPoint.Repository.LocalPath
            },
            new()
            {
                Name = "freshli:commit:id",
                Value = manifest.HistoryStopPoint.GitCommitId
            },
            new()
            {
                Name = "freshli:commit:date",
                Value = manifest.HistoryStopPoint.GitCommitDateTime.ToString("O")
            }
        });

    private async Task AddFreshliComponentProperties(Bom bom, CachedManifest manifest)
    {
        var cacheDb = await _cacheManager.GetCacheDb();
        _ = cacheDb ?? throw new Exception("CacheDb is null");

        foreach (var component in bom.Components)
        {
            var packageLibYear = manifest.PackageLibYears.Find(value => value.PackageUrl == component.Purl);
            if (packageLibYear == null)
            {
                continue;
            }

            component.Properties ??= new List<Property>();
            component.Properties.Add(new Property { Name = "freshli:libyear", Value = packageLibYear.LibYear.ToString(CultureInfo.InvariantCulture) });

            var parsedPackageUrl = new PackageURL(component.Purl);
            var releases = cacheDb.RetrieveCachedReleaseHistory(parsedPackageUrl);

            await foreach (var release in releases)
            {
                component.Properties.Add(new Property
                {
                    Name = $"freshli:release:{release.PackageUrl}",
                    Value = release.ReleasedAt.ToString("O")
                });
            }
        }
    }
}
