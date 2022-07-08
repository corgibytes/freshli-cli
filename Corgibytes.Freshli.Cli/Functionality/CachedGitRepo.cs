using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.Functionality;

[Index(nameof(Id), IsUnique = true)]
public class CachedGitRepo
{
    [Required()]
    public string Id { get; set; } = null!;
    [Required()]
    public string Url { get; set; } = null!;
    public string? Branch { get; set; }
    [Required()]
    public string LocalPath { get; set; } = null!;
}
