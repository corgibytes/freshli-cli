using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public class Configuration : IConfiguration
{
    // TODO: Remove this constant
    public const string LegacyFreshliWebApiBaseUrlEnvVarName = "FRESHLI_WEB_API_BASE_URL";
    // ReSharper disable once UnusedMember.Global
    public const string ApiServerBaseEnvVarName = "FRESHLI_API_BASE";
    // ReSharper disable once UnusedMember.Global
    public const string AuthServerBaseEnvVarName = "FRESHLI_AUTH_BASE";
    // ReSharper disable once UnusedMember.Global
    public const string AuthClientIdEnvVarName = "FRESHLI_AUTH_CLIENT_ID";

    public const string DefaultApiServerBase = "api.freshli.io";
    public const string DefaultAuthServerBase = "auth.freshli.io";
    // Note: This _is not_ a password, even though it looks like one
    public const string DefaultAuthClientId = "PzGfZ41Df9e0Dk6VpKp2kEI0uhKpggwH";

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

    public string LegacyWebApiBaseUrl
    {
        get
        {
            var valueFromEnvironment = _environment.GetVariable(LegacyFreshliWebApiBaseUrlEnvVarName);
            if (valueFromEnvironment != null)
            {
                return RemoveTrailingSlash(valueFromEnvironment);
            }

            return _freshliWebApiBaseUrl ?? "https://freshli.io";
        }

        set => _freshliWebApiBaseUrl = value != null! ? RemoveTrailingSlash(value) : value;
    }

    // TODO: allow overriding this value with an environment variable
    public string ApiServerBase { get; } = DefaultApiServerBase;
    // TODO: allow overriding this value with an environment variable
    public string AuthServerBase { get; } = DefaultAuthServerBase;
    // TODO: allow overriding this value with an environment variable
    public string AuthClientId { get; } = DefaultAuthClientId;
    public string ApiBaseUrl { get; } = $"https://{DefaultApiServerBase}/v1";

    public int WorkerCount { get; set; }
    public int AgentServiceCount => Math.Max(WorkerCount / 4, 1);

    private static string RemoveTrailingSlash(string value) =>
        value.EndsWith("/") ? value.Remove(value.Length - 1, 1) : value;
}
