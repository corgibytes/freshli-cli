using System.Text.Json;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToSnakeCase();
}
