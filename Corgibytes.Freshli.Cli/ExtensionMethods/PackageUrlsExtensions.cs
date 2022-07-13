using PackageUrl;

namespace Corgibytes.Freshli.Cli.ExtensionMethods;

public static class PackageUrlsExtensions
{
    public static bool PackageUrlEquals(this PackageURL packageUrl, PackageURL other) =>
        // Technically this isn't true equality but this what equals means to us.
        packageUrl.Name.Equals(other.Name) &&
        packageUrl.Namespace.Equals(other.Namespace) &&
        packageUrl.Version.Equals(other.Version);
}
