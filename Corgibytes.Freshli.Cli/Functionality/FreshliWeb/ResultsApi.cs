using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    private readonly IConfiguration _configuration;

    public ResultsApi(IConfiguration configuration) => _configuration = configuration;

    // TODO: the results URL should use the base URL from the configuration
    public string GetResultsUrl(Guid analysisId) => "https://freshli.io/AnalysisRequests/" + analysisId;

    public async ValueTask<Guid> CreateAnalysis(string url)
    {
        var client = new HttpClient();

        var response = await client.PostAsync(
            _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request",
            JsonContent.Create(
                new
                {
                    name = "Freshli CLI User",
                    email = "info@freshli.io",
                    url
                },
                new MediaTypeHeaderValue("application/json")
            )
        );

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var document = await response.Content.ReadFromJsonAsync<JsonNode>();
            return document!["id"]!.GetValue<Guid>();
        }

        throw new InvalidOperationException($"Failed to create analysis with url: {url}.");
    }

    public async ValueTask UpdateAnalysis(Guid apiAnalysisId, string status)
    {
        var client = new HttpClient();

        var response = await client.PutAsync(
            _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request/" + apiAnalysisId,
            JsonContent.Create(
                new { state = status },
                new MediaTypeHeaderValue("application/json")
            )
        );

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException(
                $"Failed to update analysis '{apiAnalysisId}' with state = '{status}'.");
        }
    }

    public async ValueTask CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, int historyStopPointId)
    {
        var cachedAnalysis = await cacheDb.RetrieveAnalysis(analysisId);
        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;

        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(historyStopPointId);
        var asOfDateTime = historyStopPoint!.AsOfDateTime;

        var client = new HttpClient();

        var response = await client.PostAsync(
            _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request/" + apiAnalysisId,
            JsonContent.Create(
                new { date = asOfDateTime.ToString("o") },
                new MediaTypeHeaderValue("application/json")
            )
        );

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException(
                $"Failed to create history point for analysis '{apiAnalysisId}' with '{asOfDateTime}'.");
        }
    }

    public async ValueTask CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId)
    {
        var cachedAnalysis = await cacheDb.RetrieveAnalysis(analysisId);
        var packageLibYear = await cacheDb.RetrievePackageLibYear(packageLibYearId);

        var historyStopPointId = packageLibYear!.HistoryStopPointId;
        var historyStopPoint = await cacheDb.RetrieveHistoryStopPoint(historyStopPointId);

        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;
        var asOfDateTime = historyStopPoint!.AsOfDateTime;

        var client = new HttpClient();

        var response = await client.PostAsync(
            $"{_configuration.FreshliWebApiBaseUrl}/api/v0/analysis-request/{apiAnalysisId}/{asOfDateTime:o}",
            JsonContent.Create(
                new
                {
                    packageUrl = packageLibYear.CurrentVersion!,
                    publicationDate = packageLibYear.ReleaseDateCurrentVersion.ToString("o"),
                    libYear = packageLibYear.LibYear
                },
                new MediaTypeHeaderValue("application/json")
            )
        );

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException(
                $"Failed to create package lib year for analysis '{apiAnalysisId}' and '{asOfDateTime:o}' with package URL '{packageLibYear.CurrentVersion!}' publication date '{packageLibYear.ReleaseDateCurrentVersion:o}' and LibYear '{packageLibYear.LibYear}'.");
        }
    }
}
