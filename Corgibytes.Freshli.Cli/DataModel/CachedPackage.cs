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
        PackageUrlWithoutVersion = package.PackageUrl.FormatWithoutVersion();
        Version = package.PackageUrl.Version;
        ReleasedAt = package.ReleasedAt;
    }

    [Required] public int Id { get; set; }
    [Required] public string PackageUrlWithoutVersion { get; set; } = null!;
    [Required] public string Version { get; set; } = null!;
    [Required] public DateTimeOffset ReleasedAt { get; set; }

    public Package ToPackage()
    {
        var packageUrl = new PackageURL(PackageUrlWithoutVersion);
        var packageUrlWithVersion = new PackageURL(
            packageUrl.Type, packageUrl.Namespace, packageUrl.Name, Version, packageUrl.Qualifiers, packageUrl.Subpath);

        return new Package(packageUrlWithVersion, ReleasedAt);
    }
}
