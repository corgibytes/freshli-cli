using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Functionality;
[Index(nameof(Id), IsUnique = true)]
public class CachedGitRepo
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Branch { get; set; }
    public string LocalPath { get; set; }
}
