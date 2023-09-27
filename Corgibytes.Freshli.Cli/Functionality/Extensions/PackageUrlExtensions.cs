using System;
using NLog;
using PackageUrl;

namespace Corgibytes.Freshli.Cli.Functionality.Extensions;

public static class PackageUrlExtensions
{
    public static string FormatWithoutVersion(this PackageURL packageUrl)
    {
        var other = new PackageURL(packageUrl.Type, packageUrl.Namespace, packageUrl.Name, null, null, null);
        return other.ToString()!;
    }

    private static bool AreVersionsEquivalent(string left, string right)
    {
        if (left == right)
        {
            return true;
        }

        var longer = left;
        var shorter = right;
        if (left.Length < right.Length)
        {
            longer = right;
            shorter = left;
        }

        if (longer.StartsWith(shorter))
        {
            if (longer.EndsWith(".0"))
            {
                return true;
            }
        }

        return false;
    }

    public static bool PackageUrlEquals(this PackageURL packageUrl, PackageURL? other)
    {
        // Technically this isn't true equality but this what equals means to us.
        // it's not obvious, but the == compiles the same as string.Equals(str0, str1)
        try
        {
            return other != null &&
                packageUrl.Name == other.Name &&
                packageUrl.Namespace == other.Namespace &&
                AreVersionsEquivalent(packageUrl.Version, other.Version);
        }
        catch (Exception e)
        {
            LogManager.GetCurrentClassLogger()
                .Error("Exception comparing {Purl} to {Other}: {Message}",
                    (object)packageUrl, (object)other!, (object)e.Message);
        }

        return false;
    }
}
