using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class ResultsApi : IResultsApi
{
    private readonly IConfiguration _configuration;

    public ResultsApi(IConfiguration configuration) => _configuration = configuration;

    public string GetResultsUrl(Guid analysisId) => "https://freshli.io/AnalysisRequests/" + analysisId;

    public Guid CreateAnalysis(string url)
    {
        var client = new HttpClient();

        var response = client.PostAsync(
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
        ).Result;

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var document = response.Content.ReadFromJsonAsync<JsonNode>().Result;
            return document!["id"]!.GetValue<Guid>();
        }

        throw new InvalidOperationException($"Failed to create analysis with url: {url}.");
    }

    public void CreateHistoryPoint(ICacheDb cacheDb, Guid analysisId, int historyStopPointId)
    {
        var cachedAnalysis = cacheDb.RetrieveAnalysis(analysisId);
        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;

        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(historyStopPointId);
        var asOfDateTime = historyStopPoint!.AsOfDateTime;

        var client = new HttpClient();

        var response = client.PostAsync(
            _configuration.FreshliWebApiBaseUrl + "/api/v0/analysis-request/" + apiAnalysisId,
            JsonContent.Create(
                new { date = asOfDateTime.ToString("o") },
                new MediaTypeHeaderValue("application/json")
            )
        ).Result;

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException(
                $"Failed to create history point for analysis '{apiAnalysisId}' with '{asOfDateTime}'.");
        }
    }

    public void CreatePackageLibYear(ICacheDb cacheDb, Guid analysisId, int packageLibYearId)
    {
        var cachedAnalysis = cacheDb.RetrieveAnalysis(analysisId);
        var packageLibYear = cacheDb.RetrievePackageLibYear(packageLibYearId);

        // TODO: add implementation
        // ReSharper disable once UnusedVariable
        var apiAnalysisId = cachedAnalysis!.ApiAnalysisId;

        var historyStopPointId = packageLibYear!.HistoryStopPointId;
        var historyStopPoint = cacheDb.RetrieveHistoryStopPoint(historyStopPointId);
        // ReSharper disable once UnusedVariable
        var asOfDateTime = historyStopPoint!.AsOfDateTime;
    }
}
