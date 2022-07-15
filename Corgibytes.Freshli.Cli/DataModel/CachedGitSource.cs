using Microsoft.EntityFrameworkCore;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedGitSource
{
    public CachedGitSource(string id, string url, string branch, string localPath)
    {
        Id = id;
        Url = url;
        Branch = branch;
        LocalPath = localPath;
    }

    public string Id { get; init; }
    public string Url { get; }
    public string Branch { get; }
    public string LocalPath { get; }
}
