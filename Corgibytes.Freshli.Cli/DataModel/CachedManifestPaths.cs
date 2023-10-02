using System;
using System.Security.Cryptography;
using System.Text;

namespace Corgibytes.Freshli.Cli.DataModel;

public static class CachedManifestPaths
{
    public static string GetManifestRelativeFilePath(this CachedManifest manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.HistoryStopPoint.LocalPath))
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException($"{nameof(manifest)}.{nameof(manifest.HistoryStopPoint)}.{nameof(manifest.HistoryStopPoint.LocalPath)} is not populated",
                nameof(manifest));
        }

        if (!manifest.ManifestFilePath.StartsWith(manifest.HistoryStopPoint.LocalPath))
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException($"{manifest.ManifestFilePath} is not located within {manifest.HistoryStopPoint.LocalPath}",
                nameof(manifest));
        }

        return manifest.ManifestFilePath[(manifest.HistoryStopPoint.LocalPath.Length + 1)..];
    }

    public static string GetManifestRelativeFilePathHash(this CachedManifest manifest)
    {
        var sourceManifestHash = SHA256.HashData(Encoding.UTF8.GetBytes(manifest.GetManifestRelativeFilePath()));
        return BitConverter.ToString(sourceManifestHash).Replace("-", string.Empty);
    }
}
