using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using CycloneDX.Models;
using JetBrains.Annotations;
using Moq;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public static class BillOfMaterialsProcessorTest
{
    [IntegrationTest]
    public class AnalysisMetadata : IClassFixture<ProcessorFixture>
    {
        private readonly Bom _processedBom;

        public AnalysisMetadata(ProcessorFixture fixture)
        {
            _processedBom = fixture.ProcessedBom!;
        }

        [Theory]
        [MemberData(nameof(PropertyData))]
        public void StoresProperty(string _, string expectedValue)
        {
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            Assert.Contains(_processedBom.Metadata.Properties, property =>
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
            { "freshli:commit:date", HistoryStopPoint.GitCommitDateTime.ToString("O") },
            { "freshli:manifest:path", "path/to/manifest" }
        };
    }

    [IntegrationTest]
    public class ComponentMetadata : IClassFixture<ProcessorFixture>
    {
        private readonly Bom _processedBom;

        public ComponentMetadata(ProcessorFixture fixture)
        {
            _processedBom = fixture.ProcessedBom!;
        }

        [Theory]
        [MemberData(nameof(ComponentLibYearPropertyData))]
        [MemberData(nameof(ComponentReleasePropertyData))]
        public void StoresComponentProperty(string _, string property, string expectedValue)
        {
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            Assert.Contains(_processedBom.Components, component =>
                component.Purl == _ &&
                component.Properties.Any(p => p.Name == property && p.Value == expectedValue)
            );
        }

        public static ComponentReleasePropertyTheoryData ComponentReleasePropertyData() => new()
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

        public class ComponentLibYearPropertyTheoryData : TheoryData<string, string, string>
        {
            public void Add(string purl)
            {
                base.Add(purl, "freshli:libyear", Manifest.PackageLibYears.Find(value => value.PackageUrl == purl)!.LibYear.ToString(CultureInfo.InvariantCulture));
            }
        }

        public class ComponentReleasePropertyTheoryData : TheoryData<string, string, string>
        {
            public void Add(string purl)
            {
                var releases = PackageReleases.FindAll(value => value.PackageUrlWithoutVersion == new PackageURL(purl).FormatWithoutVersion());

                foreach (var release in releases)
                {
                    base.Add(purl, $"freshli:release:{release.PackageUrl}", release.ReleasedAt.ToString("O"));
                }
            }
        }
    }

    [UsedImplicitly]
    public class ProcessorFixture : IAsyncLifetime
    {
        private BillOfMaterialsProcessor Processor { get; } = new(CacheManager.Object);
        public Bom? ProcessedBom { get; private set; }

        public async Task InitializeAsync()
        {
            var sampleBomPath = Fixtures.Path("Boms", "sample-dotnet-bom.json");
            await Processor.AddLibYearMetadataDataToBom(Manifest, AgentExecutablePath, sampleBomPath,
                cancellationToken: default);

            ProcessedBom = await LoadCycloneDxBom(sampleBomPath);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }

    private const string AgentExecutablePath = "/path/to/agent";
    private static Guid AnalysisId { get; } = Guid.NewGuid();
    private static DateTimeOffset CreatedAt { get; } = DateTimeOffset.Now;
    private static DateTimeOffset AsOfDate { get; } = DateTimeOffset.Parse("2021-01-01T00:00:00Z");

    private static Mock<ICacheManager>? s_cacheManager;
    private static Mock<ICacheManager> CacheManager
    {
        get
        {
            if (s_cacheManager != null)
            {
                return s_cacheManager;
            }

            s_cacheManager = new Mock<ICacheManager>();
            s_cacheManager.Setup(mock => mock.GetCacheDb()).ReturnsAsync(CacheDb.Object);

            return s_cacheManager;
        }
    }

    private static Mock<ICacheDb>? s_cacheDb;
    private static Mock<ICacheDb> CacheDb
    {
        get
        {
            s_cacheDb ??= new Mock<ICacheDb>();
            return s_cacheDb;
        }
    }

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
                LocalPath = "/path/to/history-stop-point"
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
                PackageLibYears = CachedPackageLibYears,
                ManifestFilePath = "path/to/manifest"
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

    class PackageReleaseList : List<CachedPackage>
    {
        private static DateTimeOffset s_lastReleasedAt = DateTimeOffset.Parse("2020-01-01T01:02:03Z");
        private static double s_lastLibYear = 1.0;

        public void Add(string rawPackageUrl)
        {
            var releaseDelta = s_lastLibYear * 365.25;

            var currentReleaseUrl = new PackageURL(rawPackageUrl);
            var packageUrlWithoutVersion = currentReleaseUrl.FormatWithoutVersion();

            var currentRelease = new CachedPackage
            {
                PackageUrl = rawPackageUrl,
                PackageUrlWithoutVersion = packageUrlWithoutVersion,
                ReleasedAt = s_lastReleasedAt
            };

            var previousReleaseUrl = DecrementVersionComponents(currentReleaseUrl);
            Assert.NotEqual(rawPackageUrl, previousReleaseUrl.ToString()!);
            var previousRelease = new CachedPackage
            {
                PackageUrl = previousReleaseUrl.ToString()!,
                PackageUrlWithoutVersion = packageUrlWithoutVersion,
                ReleasedAt = s_lastReleasedAt.AddDays(-releaseDelta)
            };

            var nextReleaseUrl = IncrementVersionComponents(currentReleaseUrl);
            Assert.NotEqual(rawPackageUrl, nextReleaseUrl.ToString()!);
            var nextRelease = new CachedPackage
            {
                PackageUrl = nextReleaseUrl.ToString()!,
                PackageUrlWithoutVersion = packageUrlWithoutVersion,
                ReleasedAt = s_lastReleasedAt.AddDays(releaseDelta)
            };

            base.Add(previousRelease);
            base.Add(currentRelease);
            base.Add(nextRelease);

            CacheDb.Setup(mock =>
                mock.RetrieveCachedReleaseHistory(
                    It.Is<PackageURL>(value => value.ToString()! == rawPackageUrl)
                )
            ).Returns(new List<CachedPackage> { previousRelease, currentRelease, nextRelease }.ToAsyncEnumerable());

            s_lastLibYear += 0.1;
            s_lastReleasedAt = s_lastReleasedAt.AddDays(1);
        }

        private static PackageURL IncrementVersionComponents(PackageURL packageUrl)
        {
            var versionComponents = packageUrl.Version.Split(".");
            var incrementedComponents = versionComponents.Select(part => int.TryParse(part, out var parsed) ? Math.Max(parsed + 1, 0).ToString() : part);
            var incrementedVersion = string.Join(".", incrementedComponents);

            return new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, incrementedVersion, packageUrl.Qualifiers, packageUrl.Subpath);
        }

        private static PackageURL DecrementVersionComponents(PackageURL packageUrl)
        {
            var versionComponents = packageUrl.Version.Split(".");
            var incrementedComponents = versionComponents.Select(part => int.TryParse(part, out var parsed) ? Math.Max(parsed - 1, 0).ToString() : part);
            var incrementedVersion = string.Join(".", incrementedComponents);

            return new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, incrementedVersion, packageUrl.Qualifiers, packageUrl.Subpath);
        }
    }

    private static List<CachedPackage>? s_packageReleases;
    private static List<CachedPackage> PackageReleases
    {
        get
        {
            s_packageReleases ??= new PackageReleaseList
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
            return s_packageReleases;
        }
    }

    private static async Task<Bom> LoadCycloneDxBom(string path)
    {
        await using var sourceBomFileStream = File.OpenRead(path);
        var bom = await CycloneDX.Json.Serializer.DeserializeAsync(sourceBomFileStream);
        return bom;
    }
}
