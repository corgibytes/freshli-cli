using PackageUrl;

namespace Corgibytes.Freshli.Cli.Extensions;

public static class PackageUrlExtensions
{
    public static string FormatWithoutVersion(this PackageURL packageUrl)
    {
        var other = new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, null, null, null);
        return other.ToString();
    }

    public static bool PackageUrlEquals(this PackageURL packageUrl, PackageURL? other) =>
        // Technically this isn't true equality but this what equals means to us.
        // it's not obvious, but the == compiles the same as string.Equals(str0, str1)
        other != null &&
        packageUrl.Name == other.Name &&
        packageUrl.Namespace == other.Namespace &&
        packageUrl.Version == other.Version;
}
