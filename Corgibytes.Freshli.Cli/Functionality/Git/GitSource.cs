using System;
using System.IO;
using System.Runtime.Serialization;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

[Serializable]
public class GitException : Exception
{
    public GitException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GitException(string message) : base(message)
    {
    }

    protected GitException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public class GitSource
{
    // ReSharper disable once UnusedMember.Global
    public GitSource(string hash, DirectoryInfo cacheDir, ICachedGitSourceRepository cachedGitSourceRepository)
    {
        // Ensure the cache directory is ready for use.
        CacheDir = cacheDir;
        Cache.Prepare(CacheDir);

        Hash = hash;

        // Get existing entry via provided hash
        var entry = cachedGitSourceRepository.FindOneByHash(hash, cacheDir);

        Url = entry.Url;
        Branch = entry.Branch;

        // Ensure the directory exists in the cache for cloning the repository.
        Directory = Cache.GetDirectoryInCache(CacheDir, new[] { "repositories", Hash });
    }

    public string Hash { get; }
    public string Url { get; }
    public string? Branch { get; }
    public DirectoryInfo Directory { get; }

    public DirectoryInfo CacheDir { get; }
}
