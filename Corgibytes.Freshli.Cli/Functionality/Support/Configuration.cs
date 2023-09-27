using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public class Configuration : IConfiguration
{
    public const string FreshliWebApiBaseUrlEnvVarName = "FRESHLI_WEB_API_BASE_URL";
    private readonly IEnvironment _environment;
    private string? _cacheDir;
    private string? _freshliWebApiBaseUrl;
    private string? _gitPath;

    public Configuration(IEnvironment environment) => _environment = environment;

    public string GitPath
    {
        get => _gitPath ?? "git";
        set => _gitPath = value;
    }

    public string CacheDir
    {
        get => _cacheDir ?? Path.Combine(_environment.HomeDirectory, ".freshli");
        set => _cacheDir = value;
    }

    public string FreshliWebApiBaseUrl
    {
        get
        {
            var valueFromEnvironment = _environment.GetVariable(FreshliWebApiBaseUrlEnvVarName);
            if (valueFromEnvironment != null)
            {
                return RemoveTrailingSlash(valueFromEnvironment);
            }

            return _freshliWebApiBaseUrl ?? "https://freshli.io";
        }

        set => _freshliWebApiBaseUrl = value != null! ? RemoveTrailingSlash(value) : value;
    }

    public int WorkerCount { get; set; }
    public int AgentServiceCount => Math.Max(WorkerCount / 4, 1);

    private static string RemoveTrailingSlash(string value) =>
        value.EndsWith("/") ? value.Remove(value.Length - 1, 1) : value;
}
