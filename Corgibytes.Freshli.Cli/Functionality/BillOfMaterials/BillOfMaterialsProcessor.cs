using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using CycloneDX.Models;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public class BillOfMaterialsProcessor : IBillOfMaterialsProcessor
{
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

        await using var bomWriteStream = File.Open(pathToBom, FileMode.Truncate);
        await CycloneDX.Json.Serializer.SerializeAsync(bom, bomWriteStream);
        await bomWriteStream.FlushAsync(cancellationToken);
        bomWriteStream.Close();
    }
}
