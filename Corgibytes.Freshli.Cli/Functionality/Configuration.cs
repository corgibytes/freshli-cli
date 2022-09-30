using System.IO;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public class Configuration : IConfiguration
{
    [JsonProperty] private IEnvironment _environment;
    [JsonProperty] private string? _gitPath;
    [JsonProperty] private string? _cacheDir;
    [JsonProperty] private string? _freshliWebApiBaseUrl;

    public const string FreshliWebApiBaseUrlEnvVarName = "FRESHLI_WEB_API_BASE_URL";

    public Configuration(IEnvironment environment)
    {
        _environment = environment;
    }

    public string GitPath
    {
        get
        {
            if (_gitPath != null)
            {
                return _gitPath;
            }

            return "git";
        }
        set
        {
            _gitPath = value;
        }
    }

    public string CacheDir
    {
        get
        {
            if (_cacheDir != null)
            {
                return _cacheDir;
            }

            return Path.Combine(_environment.HomeDirectory, ".freshli");
        }
        set
        {
            _cacheDir = value;
        }
    }

    public string FreshliWebApiBaseUrl
    {
        get
        {
            var valueFromEnvironment = _environment.GetVariable(FreshliWebApiBaseUrlEnvVarName);
            if (valueFromEnvironment != null)
            {
                return valueFromEnvironment;
            }

            if (_freshliWebApiBaseUrl != null)
            {
                return _freshliWebApiBaseUrl;
            }

            return "https://freshli.io";
        }

        set
        {
            _freshliWebApiBaseUrl = value;
        }
    }
}
