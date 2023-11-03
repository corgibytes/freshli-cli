using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Extensions;
using Corgibytes.Freshli.Cli.Functionality.Git;
using FluentAssertions;
using PackageUrl;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Cache;

[IntegrationTest]
public class CacheDbTest
{
    private TimeSpan DateTolerance { get; } = TimeSpan.FromSeconds(4);

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
            Assert.True(DateTimeOffset.Now - analysis.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - analysis.UpdatedAt < DateTolerance);
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
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = repository.Id,
                LocalPath = "localPath",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };

            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);
            Assert.NotNull(historyStopPoint);
            Assert.True(historyStopPoint.Id != 0);
            Assert.True(DateTimeOffset.Now - storedHistoryStopPoint.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - storedHistoryStopPoint.UpdatedAt < DateTolerance);
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
    public async Task RetrieveManifestWhenNull()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis()),
            };

            var savedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            const string firstManifestPath = "/path/to/history-stop-point/path/to/first/manifest";

            var retrievedManifest = await cacheDb.RetrieveManifest(savedHistoryStopPoint, firstManifestPath);
            Assert.Null(retrievedManifest);
        });
    }

    [Fact]
    public async Task RetrieveManifestAfterSave()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis()),
            };

            var savedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            const string firstManifestPath = "/path/to/history-stop-point/path/to/first/manifest";

            var savedManifest = await cacheDb.AddManifest(savedHistoryStopPoint, firstManifestPath);
            Assert.True(savedManifest.Id != 0);
            Assert.True(DateTimeOffset.Now - savedManifest.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - savedManifest.UpdatedAt < DateTolerance);
            Assert.Equal(firstManifestPath, savedManifest.ManifestFilePath);
            Assert.Equal(savedHistoryStopPoint.Id, savedManifest.HistoryStopPoint.Id);

            var retrievedManifest = await cacheDb.RetrieveManifest(savedHistoryStopPoint, firstManifestPath);
            Assert.NotNull(retrievedManifest);
            AssertManifestsEqual(savedManifest, retrievedManifest);
        });
    }

    [Fact]
    public async Task AddMultipleManifestsToHistoryStopPoint()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });
            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis()),
            };

            var savedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            const string firstManifestPath = "/path/to/history-stop-point/path/to/first/manifest";

            var savedFirstManifest = await cacheDb.AddManifest(savedHistoryStopPoint, firstManifestPath);
            Assert.True(savedFirstManifest.Id != 0);
            Assert.True(DateTimeOffset.Now - savedFirstManifest.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - savedFirstManifest.UpdatedAt < DateTolerance);
            Assert.Equal(firstManifestPath, savedFirstManifest.ManifestFilePath);
            Assert.Equal(savedHistoryStopPoint.Id, savedFirstManifest.HistoryStopPoint.Id);

            var retrievedFirstManifest = await cacheDb.RetrieveManifest(savedHistoryStopPoint, firstManifestPath);
            Assert.NotNull(retrievedFirstManifest);
            AssertManifestsEqual(savedFirstManifest, retrievedFirstManifest);

            const string secondManifestPath = "/path/to/history-stop-point/path/to/second/manifest";

            var savedSecondManifest = await cacheDb.AddManifest(savedHistoryStopPoint, secondManifestPath);
            Assert.True(savedSecondManifest.Id != 0);
            Assert.True(DateTimeOffset.Now - savedSecondManifest.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - savedSecondManifest.UpdatedAt < DateTolerance);
            Assert.Equal(secondManifestPath, savedSecondManifest.ManifestFilePath);
            Assert.Equal(savedHistoryStopPoint.Id, savedSecondManifest.HistoryStopPoint.Id);

            var retrievedSecondManifest = await cacheDb.RetrieveManifest(savedHistoryStopPoint, secondManifestPath);
            Assert.NotNull(retrievedSecondManifest);
            AssertManifestsEqual(savedSecondManifest, retrievedSecondManifest);
        });
    }

    [Fact]
    public async Task AddSecondManifestToUntrackedHistoryStopPoint()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = DateTimeOffset.Now,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis()),
            };

            var savedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);
            historyStopPoint.Id = savedHistoryStopPoint.Id;

            const string firstManifestPath = "/path/to/history-stop-point/path/to/first/manifest";

            var savedFirstManifest = await cacheDb.AddManifest(historyStopPoint, firstManifestPath);
            Assert.True(savedFirstManifest.Id != 0);
            Assert.True(DateTimeOffset.Now - savedFirstManifest.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - savedFirstManifest.UpdatedAt < DateTolerance);
            Assert.Equal(firstManifestPath, savedFirstManifest.ManifestFilePath);
            Assert.Equal(historyStopPoint.Id, savedFirstManifest.HistoryStopPoint.Id);

            var retrievedFirstManifest = await cacheDb.RetrieveManifest(historyStopPoint, firstManifestPath);
            Assert.NotNull(retrievedFirstManifest);
            AssertManifestsEqual(savedFirstManifest, retrievedFirstManifest);

            const string secondManifestPath = "/path-to-history-stop-point/path/to/second/manifest";

            var savedSecondManifest = await cacheDb.AddManifest(historyStopPoint, secondManifestPath);
            Assert.True(savedSecondManifest.Id != 0);
            Assert.True(DateTimeOffset.Now - savedSecondManifest.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - savedSecondManifest.UpdatedAt < DateTolerance);
            Assert.Equal(secondManifestPath, savedSecondManifest.ManifestFilePath);
            Assert.Equal(historyStopPoint.Id, savedSecondManifest.HistoryStopPoint.Id);

            var retrievedSecondManifest = await cacheDb.RetrieveManifest(historyStopPoint, secondManifestPath);
            Assert.NotNull(retrievedSecondManifest);
            AssertManifestsEqual(savedSecondManifest, retrievedSecondManifest);
        });
    }

    [Fact]
    public async Task RetrievePackageLibYearAfterCreation()
    {
        await WithPreparedDatabase(async cacheDb =>
        {
            var asOfDateTime = DateTimeOffset.Now;

            var analysisId = await cacheDb.SaveAnalysis(BuildCompleteCachedAnalysis());
            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = asOfDateTime,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };

            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            const string manifestFilePath = "/path/to/history-stop-point/path/to/manifest";
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
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.UpdatedAt < DateTolerance);
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

            var repository = await cacheDb.AddCachedGitSource(new CachedGitSource()
            {
                Id = "repoId",
                Url = "url",
                Branch = "branch",
                LocalPath = "/path/to/clone"
            });

            var historyStopPoint = new CachedHistoryStopPoint
            {
                AsOfDateTime = asOfDateTime,
                RepositoryId = repository.Id,
                LocalPath = "/path/to/history-stop-point",
                GitCommitId = "gitCommitId",
                CachedAnalysisId = analysisId,
            };
            var storedHistoryStopPoint = await cacheDb.AddHistoryStopPoint(historyStopPoint);

            var firstManifest = await cacheDb.AddManifest(storedHistoryStopPoint, "/path/to/history-stop-point/path/to/first/manifest");
            var secondManifest = await cacheDb.AddManifest(storedHistoryStopPoint, "/path/to/history-stop-point/path/to/second/manifest");

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
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.UpdatedAt < DateTolerance);
            cachedPackageLibYear.Id = storedPackageLibYear.Id;
            AssertCachedPackageLibYearsEqual(cachedPackageLibYear, storedPackageLibYear);
            Assert.Single(storedPackageLibYear.Manifests);

            storedPackageLibYear = await cacheDb.AddPackageLibYear(secondManifest, cachedPackageLibYear);
            Assert.True(storedPackageLibYear.Id != 0);
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.CreatedAt < DateTolerance);
            Assert.True(DateTimeOffset.Now - storedPackageLibYear.UpdatedAt < DateTolerance);
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
                await cacheDb.RetrieveManifest(storedHistoryStopPoint, "/path/to/history-stop-point/path/to/first/manifest");
            Assert.NotNull(updatedFirstManifest);
            Assert.Equal(
                retrievedPackageLibYear.Id,
                updatedFirstManifest.PackageLibYears.First().Id
            );
            updatedFirstManifest.UpdatedAt.Should().BeWithin(DateTolerance).Before(DateTimeOffset.Now);

            var updatedSecondManifest =
                await cacheDb.RetrieveManifest(storedHistoryStopPoint, "/path/to/history-stop-point/path/to/second/manifest");
            Assert.NotNull(updatedSecondManifest);
            Assert.Equal(
                retrievedPackageLibYear.Id,
                updatedSecondManifest.PackageLibYears.First().Id
            );
            updatedSecondManifest.UpdatedAt.Should().BeWithin(DateTolerance).Before(DateTimeOffset.Now);
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
                    PackageUrl = packageUrl.ToString()!,
                    PackageUrlWithoutVersion = packageUrl.FormatWithoutVersion(),
                    Version = "12.0.3",
                    ReleasedAt = DateTimeOffset.Parse("2021-01-01T00:00:00Z"),
                },
                new()
                {
                    PackageUrl = packageUrl.ToString()!,
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
                        && DateTimeOffset.Now - actualPackage.CreatedAt < DateTolerance
                        && DateTimeOffset.Now - actualPackage.UpdatedAt < DateTolerance
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
        new()
        {
            RepositoryUrl = "git@github.com:corgibytes/freshli",
            RepositoryBranch = "test",
            HistoryInterval = "1m",
            UseCommitHistory = CommitHistory.Full,
            RevisionHistoryMode = RevisionHistoryMode.AllRevisions,
            ApiAnalysisId = Guid.NewGuid()
        };

    private static void AssertManifestsEqual(CachedManifest expected, CachedManifest actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
        Assert.Equal(expected.ManifestFilePath, actual.ManifestFilePath);
        Assert.Equal(expected.HistoryStopPoint.Id, actual.HistoryStopPoint.Id);
    }

    private static void AssertCachedHistoryStopPointsEqual(CachedHistoryStopPoint expected, CachedHistoryStopPoint actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
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
