namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface IConfiguration
{
    public string GitPath { get; set; }
    public string CacheDir { get; set; }

    // ReSharper disable once UnusedMemberInSuper.Global
    public string FreshliWebApiBaseUrl { get; set; }

    public int WorkerCount { get; set; }

    public int AgentServiceCount { get; }
}
