using System;
using System.IO;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public class Configuration : IConfiguration
{
    // ReSharper disable once UnusedMember.Global
    public const string ApiServerBaseEnvVarName = "FRESHLI_API_SERVER";
    // ReSharper disable once UnusedMember.Global
    public const string AuthServerBaseEnvVarName = "FRESHLI_AUTH_SERVER";
    // ReSharper disable once UnusedMember.Global
    public const string AuthClientIdEnvVarName = "FRESHLI_AUTH_CLIENT_ID";
    public const string UiUrlEnvVarName = "FRESHLI_UI_URL";

    public const string DefaultApiServerBase = "api.freshli.io";
    public const string DefaultAuthServerBase = "auth.freshli.io";
    // Note: This _is not_ a password, even though it looks like one
    public const string DefaultAuthClientId = "PzGfZ41Df9e0Dk6VpKp2kEI0uhKpggwH";
    public const string DefaultUiUrl = "https://freshli.io";

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

    public string ApiServerBase
    {
        get
        {
            var valueFromEnvironment = _environment.GetVariable(ApiServerBaseEnvVarName);
            return valueFromEnvironment ?? DefaultApiServerBase;
        }
    }

    // TODO: allow overriding this value with an environment variable
    public string AuthServerBase { get; } = DefaultAuthServerBase;
    // TODO: allow overriding this value with an environment variable
    public string AuthClientId { get; } = DefaultAuthClientId;
    public string CanonicalApiBaseUrl { get; } = $"https://{DefaultApiServerBase}/v1";

    public string ApiBaseUrl
    {
        get
        {
            return $"https://{ApiServerBase}/v1";
        }
    }

    public int WorkerCount { get; set; }
    public int AgentServiceCount => Math.Max(WorkerCount / 4, 1);
    public string UiUrl
    {
        get
        {
            var valueFromEnvironment = RemoveTrailingSlash(_environment.GetVariable(UiUrlEnvVarName));
            return valueFromEnvironment ?? DefaultUiUrl;
        }
    }
    public string ProjectSlug { get; set; }

    private static string? RemoveTrailingSlash(string? value)
    {
        if (value == null)
        {
            return null;
        }

        return value.EndsWith("/") ? value.Remove(value.Length - 1, 1) : value;
    }
}
