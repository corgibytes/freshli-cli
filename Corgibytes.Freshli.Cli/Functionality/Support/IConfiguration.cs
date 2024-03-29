namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface IConfiguration
{
    public string GitPath { get; set; }
    public string CacheDir { get; set; }
    // TODO: Remove this property
    public string LegacyWebApiBaseUrl { get; }
    public string ApiServerBase { get; }
    public string AuthServerBase { get; }
    public string AuthClientId { get; }
    // ReSharper disable once UnusedMemberInSuper.Global
    public string ApiBaseUrl { get; }
    public int WorkerCount { get; set; }
    public int AgentServiceCount { get; }
}
