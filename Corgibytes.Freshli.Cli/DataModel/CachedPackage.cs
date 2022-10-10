using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedPackage
{
    [Required] public int Id { get; set; }
    [Required] public string PackageName { get; set; }
    [Required] public PackageURL PackageUrl { get; set; }
    [Required] public DateTimeOffset ReleasedAt { get; set; }
}
