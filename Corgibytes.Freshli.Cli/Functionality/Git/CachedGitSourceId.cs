using System.Security.Cryptography;
using System.Text;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceId
{
    public string Id { get; }

    public CachedGitSourceId(string url, string? branch = null)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url + branch));
        var stringBuilder = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            stringBuilder.Append(hashByte.ToString("x2"));
        }

        Id = stringBuilder.ToString();
    }
}

