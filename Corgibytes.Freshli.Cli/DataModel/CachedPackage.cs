using System;
using System.ComponentModel.DataAnnotations;
using Corgibytes.Freshli.Cli.Extensions;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(PackageUrlWithoutVersion))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedPackage : TimeStampedEntity
{
    public CachedPackage()
    {
    }

    public CachedPackage(Package package)
    {
        PackageUrl = package.PackageUrl.ToString()!;
        PackageUrlWithoutVersion = package.PackageUrl.FormatWithoutVersion();
        Version = package.PackageUrl.Version;
        ReleasedAt = package.ReleasedAt;
    }

    [Required] public int Id { get; set; }
    [Required] public string PackageUrl { get; init; } = null!;
    [Required] public string PackageUrlWithoutVersion { get; set; } = null!;
    [Required] public string Version { get; set; } = null!;
    [Required] public DateTimeOffset ReleasedAt { get; set; }

    public Package ToPackage()
    {
        var packageUrl = new PackageURL(PackageUrl);
        return new Package(packageUrl, ReleasedAt);
    }
}
