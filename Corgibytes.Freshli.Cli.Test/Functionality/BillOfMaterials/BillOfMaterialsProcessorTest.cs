using System;
using System.IO;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using CycloneDX.Models;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public static class BillOfMaterialsProcessorTest
{
    [IntegrationTest]
    public class AnalysisMetadata : IAsyncLifetime
    {
        private readonly BillOfMaterialsProcessor _processor = new();
        private Bom? _processedBom;

        public async Task InitializeAsync()
        {
            await _processor.AddLibYearMetadataDataToBom(Manifest, AgentExecutablePath, SourceBomFile,
                cancellationToken: default);

            _processedBom = await LoadCycloneDxBom();
        }

        [Theory]
        [MemberData(nameof(PropertyData))]
        public void StoresProperty(string _, string expectedValue)
        {
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            Assert.Contains(_processedBom!.Metadata.Properties, property =>
                property.Name == _ &&
                property.Value == expectedValue
            );
        }

        public static TheoryData<string, string> PropertyData() => new()
        {
            { "freshli:analysis:id", AnalysisId.ToString() },
            { "freshli:analysis:creation-date", CreatedAt.ToString("O") },
            { "freshli:analysis:data-point", HistoryStopPoint.AsOfDateTime.ToString("O") },
            { "freshli:source:url", HistoryStopPoint.Repository.Url },
            { "freshli:source:branch", HistoryStopPoint.Repository.Branch! },
            { "freshli:source:clone-path", HistoryStopPoint.Repository.LocalPath },
            { "freshli:commit:id", HistoryStopPoint.GitCommitId },
            { "freshli:commit:date", HistoryStopPoint.GitCommitDateTime.ToString("O") }
        };

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }

    private const string AgentExecutablePath = "/path/to/agent";
    private static string SourceBomFile { get; } = Fixtures.Path("Boms", "sample-dotnet-bom.json");
    private static Guid AnalysisId { get; } = Guid.NewGuid();
    private static DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
    private static DateTimeOffset AsOfDate { get; } = DateTimeOffset.Parse("2021-01-01T00:00:00Z");

    private static CachedAnalysis? s_analysis;

    private static CachedAnalysis Analysis
    {
        get
        {
            s_analysis ??= new CachedAnalysis()
            {
                Id = AnalysisId,
                CreatedAt = CreatedAt
            };
            return s_analysis;
        }
    }

    private static CachedGitSource? s_gitSource;

    private static CachedGitSource GitSource
    {
        get
        {
            s_gitSource ??= new CachedGitSource
            {
                Id = "source-id",
                Url = "/path/to/repo.git",
                Branch = "main",
            };
            return s_gitSource;
        }
    }

    private static CachedHistoryStopPoint? s_historyStopPoint;

    private static CachedHistoryStopPoint HistoryStopPoint
    {
        get
        {
            s_historyStopPoint ??= new CachedHistoryStopPoint
            {
                Id = 29,
                CachedAnalysis = Analysis,
                AsOfDateTime = AsOfDate,
                Repository = GitSource
            };
            return s_historyStopPoint;
        }
    }

    private static CachedManifest? s_manifest;

    private static CachedManifest Manifest
    {
        get
        {
            s_manifest ??= new CachedManifest { HistoryStopPoint = HistoryStopPoint };
            return s_manifest;
        }
    }

    private static async Task<Bom> LoadCycloneDxBom()
    {
        await using var sourceBomFileStream = File.OpenRead(SourceBomFile);
        var bom = await CycloneDX.Json.Serializer.DeserializeAsync(sourceBomFileStream);
        return bom;
    }
}
