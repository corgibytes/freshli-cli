using System.Security.Cryptography;
using System.Text;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class CachedGitSourceId
{
    public CachedGitSourceId(string url, string? branch = null)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(url + branch));
        var stringBuilder = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            stringBuilder.Append(hashByte.ToString("x2"));
        }

        Id = stringBuilder.ToString();
    }

    public CachedGitSourceId(string id) => Id = id;

    public string Id { get; }
}
