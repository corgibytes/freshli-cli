using System.Text;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public static class SnakeCaseStrings
{
    public static string ToSnakeCase(this string value)
    {
        var builder = new StringBuilder();

        var first = true;
        foreach (var character in value)
        {
            if (char.IsUpper(character))
            {
                if (!first)
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
            }
            else
            {
                builder.Append(character);
            }
            first = false;
        }

        return builder.ToString();
    }
}
