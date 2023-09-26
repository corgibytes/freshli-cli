using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

    [IntegrationTest]
    public class ComponentMetadata : IAsyncLifetime
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
        [MemberData(nameof(ComponentLibYearPropertyData))]
        public void StoresComponentProperty(string _, string property, string expectedValue)
        {
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            Assert.Contains(_processedBom!.Components, component =>
                component.Purl == _ &&
                component.Properties.Any(p => p.Name == property && p.Value == expectedValue)
            );
        }

        public class ComponentLibYearPropertyTheoryData : TheoryData<string, string, string>
        {
            public void Add(string purl)
            {
                base.Add(purl, "freshli:libyear", Manifest.PackageLibYears.Find(value => value.PackageUrl == purl)!.LibYear.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static ComponentLibYearPropertyTheoryData ComponentLibYearPropertyData() => new()
        {
            "pkg:nuget/DotNetEnv@1.4.0",
            "pkg:nuget/Elasticsearch.Net@7.10.1",
            "pkg:nuget/HtmlAgilityPack@1.11.30",
            "pkg:nuget/LibGit2Sharp@0.27.0-preview-0096",
            "pkg:nuget/LibGit2Sharp.NativeBinaries@2.0.312",
            "pkg:nuget/Microsoft.CSharp@4.6.0",
            "pkg:nuget/Microsoft.NETCore.Platforms@1.1.0",
            "pkg:nuget/Microsoft.NETCore.Targets@1.1.0",
            "pkg:nuget/Microsoft.Win32.Primitives@4.3.0",
            "pkg:nuget/NETStandard.Library@1.6.1",
            "pkg:nuget/NLog@4.7.7",
            "pkg:nuget/RestSharp@106.11.7",
            "pkg:nuget/runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.native.System@4.3.0",
            "pkg:nuget/runtime.native.System.IO.Compression@4.3.0",
            "pkg:nuget/runtime.native.System.Net.Http@4.3.0",
            "pkg:nuget/runtime.native.System.Security.Cryptography.Apple@4.3.0",
            "pkg:nuget/runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple@4.3.0",
            "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/System.AppContext@4.3.0",
            "pkg:nuget/System.Buffers@4.5.1",
            "pkg:nuget/System.Collections@4.3.0",
            "pkg:nuget/System.Collections.Concurrent@4.3.0",
            "pkg:nuget/System.Console@4.3.0",
            "pkg:nuget/System.Diagnostics.Debug@4.3.0",
            "pkg:nuget/System.Diagnostics.DiagnosticSource@5.0.0",
            "pkg:nuget/System.Diagnostics.Tools@4.3.0",
            "pkg:nuget/System.Diagnostics.Tracing@4.3.0",
            "pkg:nuget/System.Globalization@4.3.0",
            "pkg:nuget/System.Globalization.Calendars@4.3.0",
            "pkg:nuget/System.Globalization.Extensions@4.3.0",
            "pkg:nuget/System.IO@4.3.0",
            "pkg:nuget/System.IO.Compression@4.3.0",
            "pkg:nuget/System.IO.Compression.ZipFile@4.3.0",
            "pkg:nuget/System.IO.FileSystem@4.3.0",
            "pkg:nuget/System.IO.FileSystem.Primitives@4.3.0",
            "pkg:nuget/System.Linq@4.3.0",
            "pkg:nuget/System.Linq.Expressions@4.3.0",
            "pkg:nuget/System.Net.Http@4.3.0",
            "pkg:nuget/System.Net.Primitives@4.3.0",
            "pkg:nuget/System.Net.Sockets@4.3.0",
            "pkg:nuget/System.ObjectModel@4.3.0",
            "pkg:nuget/System.Reflection@4.3.0",
            "pkg:nuget/System.Reflection.Emit@4.3.0",
            "pkg:nuget/System.Reflection.Emit.ILGeneration@4.3.0",
            "pkg:nuget/System.Reflection.Emit.Lightweight@4.3.0",
            "pkg:nuget/System.Reflection.Extensions@4.3.0",
            "pkg:nuget/System.Reflection.Primitives@4.3.0",
            "pkg:nuget/System.Reflection.TypeExtensions@4.3.0",
            "pkg:nuget/System.Resources.ResourceManager@4.3.0",
            "pkg:nuget/System.Runtime@4.3.0",
            "pkg:nuget/System.Runtime.Extensions@4.3.0",
            "pkg:nuget/System.Runtime.Handles@4.3.0",
            "pkg:nuget/System.Runtime.InteropServices@4.3.0",
            "pkg:nuget/System.Runtime.InteropServices.RuntimeInformation@4.3.0",
            "pkg:nuget/System.Runtime.Numerics@4.3.0",
            "pkg:nuget/System.Security.Cryptography.Algorithms@4.3.0",
            "pkg:nuget/System.Security.Cryptography.Cng@4.3.0",
            "pkg:nuget/System.Security.Cryptography.Csp@4.3.0",
            "pkg:nuget/System.Security.Cryptography.Encoding@4.3.0",
            "pkg:nuget/System.Security.Cryptography.OpenSsl@4.3.0",
            "pkg:nuget/System.Security.Cryptography.Primitives@4.3.0",
            "pkg:nuget/System.Security.Cryptography.X509Certificates@4.3.0",
            "pkg:nuget/System.Text.Encoding@4.3.0",
            "pkg:nuget/System.Text.Encoding.Extensions@4.3.0",
            "pkg:nuget/System.Text.RegularExpressions@4.3.0",
            "pkg:nuget/System.Threading@4.3.0",
            "pkg:nuget/System.Threading.Tasks@4.3.0",
            "pkg:nuget/System.Threading.Tasks.Extensions@4.3.0",
            "pkg:nuget/System.Threading.Timer@4.3.0",
            "pkg:nuget/System.Xml.ReaderWriter@4.3.0",
            "pkg:nuget/System.Xml.XDocument@4.3.0"
        };

        // [
        // "pkg:nuget/DotNetEnv@1.4.0",
        // "pkg:nuget/Elasticsearch.Net@7.10.1",
        // "pkg:nuget/HtmlAgilityPack@1.11.30",
        // "pkg:nuget/LibGit2Sharp@0.27.0-preview-0096",
        // "pkg:nuget/LibGit2Sharp.NativeBinaries@2.0.312",
        // "pkg:nuget/Microsoft.CSharp@4.6.0",
        // "pkg:nuget/Microsoft.NETCore.Platforms@1.1.0",
        // "pkg:nuget/Microsoft.NETCore.Targets@1.1.0",
        // "pkg:nuget/Microsoft.Win32.Primitives@4.3.0",
        // "pkg:nuget/NETStandard.Library@1.6.1",
        // "pkg:nuget/NLog@4.7.7",
        // "pkg:nuget/RestSharp@106.11.7",
        // "pkg:nuget/runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.native.System@4.3.0",
        // "pkg:nuget/runtime.native.System.IO.Compression@4.3.0",
        // "pkg:nuget/runtime.native.System.Net.Http@4.3.0",
        // "pkg:nuget/runtime.native.System.Security.Cryptography.Apple@4.3.0",
        // "pkg:nuget/runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple@4.3.0",
        // "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/System.AppContext@4.3.0",
        // "pkg:nuget/System.Buffers@4.5.1",
        // "pkg:nuget/System.Collections@4.3.0",
        // "pkg:nuget/System.Collections.Concurrent@4.3.0",
        // "pkg:nuget/System.Console@4.3.0",
        // "pkg:nuget/System.Diagnostics.Debug@4.3.0",
        // "pkg:nuget/System.Diagnostics.DiagnosticSource@5.0.0",
        // "pkg:nuget/System.Diagnostics.Tools@4.3.0",
        // "pkg:nuget/System.Diagnostics.Tracing@4.3.0",
        // "pkg:nuget/System.Globalization@4.3.0",
        // "pkg:nuget/System.Globalization.Calendars@4.3.0",
        // "pkg:nuget/System.Globalization.Extensions@4.3.0",
        // "pkg:nuget/System.IO@4.3.0",
        // "pkg:nuget/System.IO.Compression@4.3.0",
        // "pkg:nuget/System.IO.Compression.ZipFile@4.3.0",
        // "pkg:nuget/System.IO.FileSystem@4.3.0",
        // "pkg:nuget/System.IO.FileSystem.Primitives@4.3.0",
        // "pkg:nuget/System.Linq@4.3.0",
        // "pkg:nuget/System.Linq.Expressions@4.3.0",
        // "pkg:nuget/System.Net.Http@4.3.0",
        // "pkg:nuget/System.Net.Primitives@4.3.0",
        // "pkg:nuget/System.Net.Sockets@4.3.0",
        // "pkg:nuget/System.ObjectModel@4.3.0",
        // "pkg:nuget/System.Reflection@4.3.0",
        // "pkg:nuget/System.Reflection.Emit@4.3.0",
        // "pkg:nuget/System.Reflection.Emit.ILGeneration@4.3.0",
        // "pkg:nuget/System.Reflection.Emit.Lightweight@4.3.0",
        // "pkg:nuget/System.Reflection.Extensions@4.3.0",
        // "pkg:nuget/System.Reflection.Primitives@4.3.0",
        // "pkg:nuget/System.Reflection.TypeExtensions@4.3.0",
        // "pkg:nuget/System.Resources.ResourceManager@4.3.0",
        // "pkg:nuget/System.Runtime@4.3.0",
        // "pkg:nuget/System.Runtime.Extensions@4.3.0",
        // "pkg:nuget/System.Runtime.Handles@4.3.0",
        // "pkg:nuget/System.Runtime.InteropServices@4.3.0",
        // "pkg:nuget/System.Runtime.InteropServices.RuntimeInformation@4.3.0",
        // "pkg:nuget/System.Runtime.Numerics@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.Algorithms@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.Cng@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.Csp@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.Encoding@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.OpenSsl@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.Primitives@4.3.0",
        // "pkg:nuget/System.Security.Cryptography.X509Certificates@4.3.0",
        // "pkg:nuget/System.Text.Encoding@4.3.0",
        // "pkg:nuget/System.Text.Encoding.Extensions@4.3.0",
        // "pkg:nuget/System.Text.RegularExpressions@4.3.0",
        // "pkg:nuget/System.Threading@4.3.0",
        // "pkg:nuget/System.Threading.Tasks@4.3.0",
        // "pkg:nuget/System.Threading.Tasks.Extensions@4.3.0",
        // "pkg:nuget/System.Threading.Timer@4.3.0",
        // "pkg:nuget/System.Xml.ReaderWriter@4.3.0",
        // "pkg:nuget/System.Xml.XDocument@4.3.0"
        // ]

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
                Repository = GitSource,
            };
            return s_historyStopPoint;
        }
    }

    private static CachedManifest? s_manifest;

    private static CachedManifest Manifest
    {
        get
        {
            s_manifest ??= new CachedManifest
            {
                HistoryStopPoint = HistoryStopPoint,
                PackageLibYears = CachedPackageLibYears
            };
            return s_manifest;
        }
    }

    private class CachedPackageLibYearList : List<CachedPackageLibYear>
    {
        private static double s_lastLibYear = 1.0;

        public void Add(string packageUrl)
        {
            Add(new CachedPackageLibYear
            {
                PackageUrl = packageUrl,
                LibYear = s_lastLibYear
            });
            s_lastLibYear += 0.1;
        }
    }

    private static List<CachedPackageLibYear>? s_cachedPackageLibYears;
    private static List<CachedPackageLibYear> CachedPackageLibYears
    {
        get
        {
            s_cachedPackageLibYears ??= new CachedPackageLibYearList
            {
                "pkg:nuget/DotNetEnv@1.4.0",
                "pkg:nuget/Elasticsearch.Net@7.10.1",
                "pkg:nuget/HtmlAgilityPack@1.11.30",
                "pkg:nuget/LibGit2Sharp@0.27.0-preview-0096",
                "pkg:nuget/LibGit2Sharp.NativeBinaries@2.0.312",
                "pkg:nuget/Microsoft.CSharp@4.6.0",
                "pkg:nuget/Microsoft.NETCore.Platforms@1.1.0",
                "pkg:nuget/Microsoft.NETCore.Targets@1.1.0",
                "pkg:nuget/Microsoft.Win32.Primitives@4.3.0",
                "pkg:nuget/NETStandard.Library@1.6.1",
                "pkg:nuget/NLog@4.7.7",
                "pkg:nuget/RestSharp@106.11.7",
                "pkg:nuget/runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.native.System@4.3.0",
                "pkg:nuget/runtime.native.System.IO.Compression@4.3.0",
                "pkg:nuget/runtime.native.System.Net.Http@4.3.0",
                "pkg:nuget/runtime.native.System.Security.Cryptography.Apple@4.3.0",
                "pkg:nuget/runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple@4.3.0",
                "pkg:nuget/runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/System.AppContext@4.3.0",
                "pkg:nuget/System.Buffers@4.5.1",
                "pkg:nuget/System.Collections@4.3.0",
                "pkg:nuget/System.Collections.Concurrent@4.3.0",
                "pkg:nuget/System.Console@4.3.0",
                "pkg:nuget/System.Diagnostics.Debug@4.3.0",
                "pkg:nuget/System.Diagnostics.DiagnosticSource@5.0.0",
                "pkg:nuget/System.Diagnostics.Tools@4.3.0",
                "pkg:nuget/System.Diagnostics.Tracing@4.3.0",
                "pkg:nuget/System.Globalization@4.3.0",
                "pkg:nuget/System.Globalization.Calendars@4.3.0",
                "pkg:nuget/System.Globalization.Extensions@4.3.0",
                "pkg:nuget/System.IO@4.3.0",
                "pkg:nuget/System.IO.Compression@4.3.0",
                "pkg:nuget/System.IO.Compression.ZipFile@4.3.0",
                "pkg:nuget/System.IO.FileSystem@4.3.0",
                "pkg:nuget/System.IO.FileSystem.Primitives@4.3.0",
                "pkg:nuget/System.Linq@4.3.0",
                "pkg:nuget/System.Linq.Expressions@4.3.0",
                "pkg:nuget/System.Net.Http@4.3.0",
                "pkg:nuget/System.Net.Primitives@4.3.0",
                "pkg:nuget/System.Net.Sockets@4.3.0",
                "pkg:nuget/System.ObjectModel@4.3.0",
                "pkg:nuget/System.Reflection@4.3.0",
                "pkg:nuget/System.Reflection.Emit@4.3.0",
                "pkg:nuget/System.Reflection.Emit.ILGeneration@4.3.0",
                "pkg:nuget/System.Reflection.Emit.Lightweight@4.3.0",
                "pkg:nuget/System.Reflection.Extensions@4.3.0",
                "pkg:nuget/System.Reflection.Primitives@4.3.0",
                "pkg:nuget/System.Reflection.TypeExtensions@4.3.0",
                "pkg:nuget/System.Resources.ResourceManager@4.3.0",
                "pkg:nuget/System.Runtime@4.3.0",
                "pkg:nuget/System.Runtime.Extensions@4.3.0",
                "pkg:nuget/System.Runtime.Handles@4.3.0",
                "pkg:nuget/System.Runtime.InteropServices@4.3.0",
                "pkg:nuget/System.Runtime.InteropServices.RuntimeInformation@4.3.0",
                "pkg:nuget/System.Runtime.Numerics@4.3.0",
                "pkg:nuget/System.Security.Cryptography.Algorithms@4.3.0",
                "pkg:nuget/System.Security.Cryptography.Cng@4.3.0",
                "pkg:nuget/System.Security.Cryptography.Csp@4.3.0",
                "pkg:nuget/System.Security.Cryptography.Encoding@4.3.0",
                "pkg:nuget/System.Security.Cryptography.OpenSsl@4.3.0",
                "pkg:nuget/System.Security.Cryptography.Primitives@4.3.0",
                "pkg:nuget/System.Security.Cryptography.X509Certificates@4.3.0",
                "pkg:nuget/System.Text.Encoding@4.3.0",
                "pkg:nuget/System.Text.Encoding.Extensions@4.3.0",
                "pkg:nuget/System.Text.RegularExpressions@4.3.0",
                "pkg:nuget/System.Threading@4.3.0",
                "pkg:nuget/System.Threading.Tasks@4.3.0",
                "pkg:nuget/System.Threading.Tasks.Extensions@4.3.0",
                "pkg:nuget/System.Threading.Timer@4.3.0",
                "pkg:nuget/System.Xml.ReaderWriter@4.3.0",
                "pkg:nuget/System.Xml.XDocument@4.3.0"
            };
            return s_cachedPackageLibYears;
        }
    }

    private static async Task<Bom> LoadCycloneDxBom()
    {
        await using var sourceBomFileStream = File.OpenRead(SourceBomFile);
        var bom = await CycloneDX.Json.Serializer.DeserializeAsync(sourceBomFileStream);
        return bom;
    }
}
