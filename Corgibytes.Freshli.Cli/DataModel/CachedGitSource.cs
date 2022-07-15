using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedGitSource
{
    public CachedGitSource(string id, string url, string? branch, string localPath)
    {
        Id = id;
        Url = url;
        Branch = branch;
        LocalPath = localPath;
    }

    [Required] public string Id { get; set; } = null!;

    [Required] public string Url { get; set; } = null!;

    public string? Branch { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    [Required] public string LocalPath { get; set; } = null!;

}



