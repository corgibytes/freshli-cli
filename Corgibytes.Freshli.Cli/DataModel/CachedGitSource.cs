using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedGitSource : TimeStampedEntity
{
    [Required] public string Id { get; set; } = null!;
    [Required] public string Url { get; set; } = null!;
    // TODO: Make this field required. It should be set to the name of the default branch instead of null.
    public string? Branch { get; set; }
    // TODO: Add validation to ensure that this is not a relative path.
    [Required] public string LocalPath { get; set; } = null!;
}
