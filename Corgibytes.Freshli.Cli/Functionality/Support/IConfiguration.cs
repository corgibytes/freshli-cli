namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface IConfiguration
{
    public string GitPath { get; set; }
    public string CacheDir { get; set; }
    // ReSharper disable once UnusedMemberInSuper.Global
    public string ApiServerBase { get; }
    public string AuthServerBase { get; }
    public string AuthClientId { get; }
    // ReSharper disable once UnusedMemberInSuper.Global
    public string ApiBaseUrl { get; }
    public string CanonicalApiBaseUrl { get; }
    public int WorkerCount { get; set; }
    public int AgentServiceCount { get; }

    public string UiUrl { get; }
    public string? ProjectSlug { get; set; }
}
