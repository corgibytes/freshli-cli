using System.IO;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Configuration : IConfiguration
{
    public const string FreshliWebApiBaseUrlEnvVarName = "FRESHLI_WEB_API_BASE_URL";
    [JsonProperty] private readonly IEnvironment _environment;
    [JsonProperty] private string? _cacheDir;
    [JsonProperty] private string? _freshliWebApiBaseUrl;
    [JsonProperty] private string? _gitPath;

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

    private static string RemoveTrailingSlash(string value) =>
        value.EndsWith("/") ? value.Remove(value.Length - 1, 1) : value;
}
