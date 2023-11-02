using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedGitSource : TimeStampedEntity
{
    [Required] public string Id { get; set; } = null!;
    [Required] public string Url { get; set; } = null!;
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public string Branch { get; set; } = null!;
    // TODO: Add validation to ensure that this is not a relative path.
    [Required] public string LocalPath { get; set; } = null!;
}
