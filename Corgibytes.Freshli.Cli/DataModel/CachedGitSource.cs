using Microsoft.EntityFrameworkCore;

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

    public string Id { get; set; }
    public string Url { get; set; }
    public string Branch { get; set; }
    public string LocalPath { get; set; }
}
