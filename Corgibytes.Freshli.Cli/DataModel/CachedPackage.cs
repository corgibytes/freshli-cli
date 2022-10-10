using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PackageUrl;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedPackage
{
    [Required] public int Id { get; set; }
    [Required] public string PackageName { get; set; } = null!;
    [Required] public PackageURL PackageUrl { get; set; } = null!;
    [Required] public DateTimeOffset ReleasedAt { get; set; }
}
