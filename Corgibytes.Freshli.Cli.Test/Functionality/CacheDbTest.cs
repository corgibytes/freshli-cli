using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Functionality.Git;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

[IntegrationTest]
public class CacheDbTest
{
    [Fact]
    public async Task RetrieveAnalysisWhenNull()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var cachedAnalysis = await cacheDb.RetrieveAnalysis(Guid.NewGuid());
            Assert.Null(cachedAnalysis);
        });
    }

    [Fact]
    public async Task RetrieveAnalysisAfterCreation()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var analysis = BuildCompleteCachedAnalysis();

            var analysisId = await cacheDb.SaveAnalysis(analysis);
            Assert.True(analysisId != Guid.Empty);

            var retrievedAnalysis = await cacheDb.RetrieveAnalysis(analysisId);

            Assert.NotNull(retrievedAnalysis);
            Assert.Equal(analysisId, retrievedAnalysis.Id);
            Assert.Equal(analysis.RepositoryUrl, retrievedAnalysis.RepositoryUrl);
            Assert.Equal(analysis.RepositoryBranch, retrievedAnalysis.RepositoryBranch);
            Assert.Equal(analysis.UseCommitHistory, retrievedAnalysis.UseCommitHistory);
            Assert.Equal(analysis.RevisionHistoryMode, retrievedAnalysis.RevisionHistoryMode);
            Assert.Equal(analysis.HistoryInterval, retrievedAnalysis.HistoryInterval);
            Assert.Equal(analysis.ApiAnalysisId, retrievedAnalysis.ApiAnalysisId);
        });
    }

    [Fact]
    public async Task RetrieveHistoryStopPointWhenNull()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var cachedHistoryStopPoint = await cacheDb.RetrieveHistoryStopPoint(1);
            Assert.Null(cachedHistoryStopPoint);
        });
    }

    [Fact]
    public async Task RetrieveHistoryStopPointAfterCreation()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var analysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis());
            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = "repoId",
                LocalPath = "localPath",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };

            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);
            Assert.NotNull(historyStopPoint);
            Assert.True(historyStopPoint.Id != 0);
            AssertCachedHistoryStopPointsEqual(historyStopPoint, storedHistoryStopPoint);

            var retrievedHistoryStopPoint = await cacheDb.RetrieveHistoryStopPoint(historyStopPoint.Id);

            Assert.NotNull(retrievedHistoryStopPoint);
            AssertCachedHistoryStopPointsEqual(historyStopPoint, retrievedHistoryStopPoint);
        });
    }

    [Fact]
    public async Task RetrieveCachedGitSourceWhenNull()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var cachedGitSource = await cacheDb.RetrieveCachedGitSource(new CachedGitSourceId("repoId", "localPath"));
            Assert.Null(cachedGitSource);
        });
    }

    [Fact]
    public async Task RetrieveCachedGitSourceAfterCreation()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var gitSourceId = new CachedGitSourceId("url", "branch");
            var gitSource = new CachedGitSource
            {
                Id = gitSourceId.Id,
                Url = "url",
                Branch = "branch",
                LocalPath = "localPath"
            };

            await cacheDb.AddCachedGitSource(gitSource);

            var retrievedGitSource = await cacheDb.RetrieveCachedGitSource(gitSourceId);

            Assert.NotNull(retrievedGitSource);
            Assert.Equal(gitSource.Id, retrievedGitSource.Id);
            Assert.Equal(gitSource.Url, retrievedGitSource.Url);
            Assert.Equal(gitSource.Branch, retrievedGitSource.Branch);
            Assert.Equal(gitSource.LocalPath, retrievedGitSource.LocalPath);
        });
    }

    [Fact]
    public async Task RetrievePackageLibYearWhenNull()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var package = new PackageURL("pkg:nuget/Newtonsoft.Json@12.0.3");
            var asOfDateTime = DateTimeOffset.Now;
            var cachedPackageLibYear = await cacheDb.RetrievePackageLibYear(package, asOfDateTime);
            Assert.Null(cachedPackageLibYear);
        });
    }

    [Fact]
    public async Task RetrievePackageLibYearAfterCreation()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var asOfDateTime = DateTimeOffset.Now;

            var analysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis());
            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = asOfDateTime,
                RepositoryId = "repoId",
                LocalPath = "localPath",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };

            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            var manifestFilePath = "manifestFilePath";
            var storedManifest = await cacheDb.AddManifest(storedHistoryStopPoint, manifestFilePath);

            var package = new PackageURL("pkg:nuget/Newtonsoft.Json@12.0.3");

            var cachedPackageLibYear = new CachedPackageLibYear
            {
                PackageUrl = package.ToString()!,
                AsOfDateTime = asOfDateTime,
                ReleaseDateCurrentVersion = DateTimeOffset.Parse("2021-01-01T00:00:00Z"),
                LatestVersion = "15.9.2",
                ReleaseDateLatestVersion = DateTimeOffset.Parse("2022-01-01T00:00:00Z"),
            };

            var storedPackageLibYear = await cacheDb.AddPackageLibYear(storedManifest, cachedPackageLibYear);
            Assert.True(storedHistoryStopPoint.Id != 0);
            cachedPackageLibYear.Id = storedPackageLibYear.Id;
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, storedPackageLibYear);

            var retrievedPackageLibYear = await cacheDb.RetrievePackageLibYear(package, asOfDateTime);
            Assert.NotNull(retrievedPackageLibYear);
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, retrievedPackageLibYear);

            Assert.Equal(
                storedManifest.Id,
                retrievedPackageLibYear.Manifests.First().Id
            );

            var updatedManifest = await cacheDb.RetrieveManifest(historyStopPoint, manifestFilePath);
            Assert.NotNull(updatedManifest);
            Assert.Equal(
                retrievedPackageLibYear.Id,
                updatedManifest.PackageLibYears.First().Id
            );
        });
    }

    [Fact]
    public async Task SavePackageLibYearToSecondManifest()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var asOfDateTime = DateTimeOffset.Now;

            var analysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis());

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = asOfDateTime,
                RepositoryId = "repoId",
                LocalPath = "localPath",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };
            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            var firstManifest = await cacheDb.AddManifest(storedHistoryStopPoint, "/path/to/first/manifest");
            var secondManifest = await cacheDb.AddManifest(historyStopPoint, "/path/to/second/manifest");

            var package = new PackageURL("pkg:nuget/Newtonsoft.Json@12.0.3");

            var cachedPackageLibYear = new CachedPackageLibYear
            {
                PackageUrl = package.ToString()!,
                AsOfDateTime = asOfDateTime,
                ReleaseDateCurrentVersion = DateTimeOffset.Parse("2021-01-01T00:00:00Z"),
                LatestVersion = "15.9.2",
                ReleaseDateLatestVersion = DateTimeOffset.Parse("2022-01-01T00:00:00Z"),
            };

            var storedPackageLibYear = await cacheDb.AddPackageLibYear(firstManifest, cachedPackageLibYear);
            Assert.True(storedPackageLibYear.Id != 0);
            cachedPackageLibYear.Id = storedPackageLibYear.Id;
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, storedPackageLibYear);
            Assert.Single(storedPackageLibYear.Manifests);

            storedPackageLibYear = await cacheDb.AddPackageLibYear(secondManifest, cachedPackageLibYear);
            Assert.True(storedPackageLibYear.Id != 0);
            cachedPackageLibYear.Id = storedPackageLibYear.Id;
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, storedPackageLibYear);
            Assert.Equal(2, storedPackageLibYear.Manifests.Count);

            var retrievedPackageLibYear = await cacheDb.RetrievePackageLibYear(package, asOfDateTime);
            Assert.NotNull(retrievedPackageLibYear);
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, retrievedPackageLibYear);

            Assert.Equal(
                new[] { firstManifest.Id, secondManifest.Id },
                retrievedPackageLibYear.Manifests.Select(value => value.Id).ToList().Order().ToArray()
            );

            var updatedFirstManifest =
                await cacheDb.RetrieveManifest(storedHistoryStopPoint, "/path/to/first/manifest");
            Assert.NotNull(updatedFirstManifest);
            Assert.Equal(
                retrievedPackageLibYear.Id,
                updatedFirstManifest.PackageLibYears.First().Id
            );

            var updatedSecondManifest =
                await cacheDb.RetrieveManifest(storedHistoryStopPoint, "/path/to/second/manifest");
            Assert.NotNull(updatedSecondManifest);
            Assert.Equal(
                retrievedPackageLibYear.Id,
                updatedSecondManifest.PackageLibYears.First().Id
            );

        });
    }

    [Fact]
    public async Task RetrieveCachedReleaseHistoryWhenEmpty()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var packageUrl = new PackageURL("pkg:nuget/Newtonsoft.Json@12.0.3");
            var results = await cacheDb.RetrieveCachedReleaseHistory(packageUrl).ToListAsync();
            Assert.Empty(results);
        });
    }

    [Fact]
    public async Task RetrieveCachedReleaseHistoryAfterStore()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var packageUrl = new PackageURL("pkg:nuget/Newtonsoft.Json@12.0.3");
            var expectedPackages = new List<CachedPackage>
            {
                new()
                {
                    PackageUrlWithoutVersion = packageUrl.FormatWithoutVersion(),
                    Version = "12.0.3",
                    ReleasedAt = DateTimeOffset.Parse("2021-01-01T00:00:00Z"),
                },
                new()
                {
                    PackageUrlWithoutVersion = packageUrl.FormatWithoutVersion(),
                    Version = "15.9.2",
                    ReleasedAt = DateTimeOffset.Parse("2022-01-01T00:00:00Z"),
                },
            };

            await cacheDb.StoreCachedReleaseHistory(expectedPackages);

            var actualPackages = await cacheDb.RetrieveCachedReleaseHistory(packageUrl).ToListAsync();
            Assert.Equal(expectedPackages.Count, actualPackages.Count);
            foreach (var expectedPackage in expectedPackages)
            {
                Assert.True(
                    actualPackages.Exists(actualPackage =>
                        actualPackage.Id != 0
                        && expectedPackage.PackageUrlWithoutVersion == actualPackage.PackageUrlWithoutVersion
                        && expectedPackage.Version == actualPackage.Version
                        && expectedPackage.ReleasedAt == actualPackage.ReleasedAt
                    )
                );
            }
        });
    }

    private static async Task WithPreparedDatabase(Func<CacheDb, Task> action)
    {
        var tempDb = Path.GetTempFileName();
        File.Delete(tempDb);
        Directory.CreateDirectory(tempDb);

        await using var cacheContext = new CacheContext(tempDb);
        await cacheContext.Database.EnsureCreatedAsync();

        try
        {
            await using var cacheDb = new CacheDb(tempDb);
            await action(cacheDb);
        }
        finally
        {
            Directory.Delete(tempDb, true);
        }
    }

    private static CachedAnalysis BuildCompleteCachedAnalysis() =>
        new(
            "git@github.com:corgibytes/freshli", "test", "1m", CommitHistory.Full,
            RevisionHistoryMode.AllRevisions
        )
        {
            ApiAnalysisId = Guid.NewGuid()
        };

    private static void AssertCachedHistoryStopPointsEqual(CachedHistoryStopPoint expected, CachedHistoryStopPoint actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.AsOfDateTime, actual.AsOfDateTime);
        Assert.Equal(expected.RepositoryId, actual.RepositoryId);
        Assert.Equal(expected.LocalPath, actual.LocalPath);
        Assert.Equal(expected.GitCommitId, actual.GitCommitId);
        Assert.Equal(expected.CachedAnalysisId, actual.CachedAnalysisId);
    }

    private static void AssertCachedPackageLibYearsEqual(CachedPackageLibYear expected, CachedPackageLibYear actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.PackageUrl, actual.PackageUrl);
        Assert.Equal(expected.AsOfDateTime, actual.AsOfDateTime);
        Assert.Equal(expected.ReleaseDateCurrentVersion, actual.ReleaseDateCurrentVersion);
        Assert.Equal(expected.LatestVersion, actual.LatestVersion);
        Assert.Equal(expected.ReleaseDateLatestVersion, actual.ReleaseDateLatestVersion);
    }
}
