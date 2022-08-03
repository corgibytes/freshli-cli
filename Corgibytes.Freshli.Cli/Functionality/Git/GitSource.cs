using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Repositories;
using Corgibytes.Freshli.Cli.Resources;

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
    private ICacheManager CacheManager { get; }

    // ReSharper disable once UnusedMember.Global
    public GitSource(string hash, string cacheDirPath, ICacheManager cacheManager, ICachedGitSourceRepository cachedGitSourceRepository)
    {
        CacheManager = cacheManager;
        CacheDir = new(cacheDirPath);
        CacheManager.Prepare(cacheDirPath);

        Hash = hash;

        // Get existing entry via provided hash
        var entry = cachedGitSourceRepository.FindOneByHash(hash, CacheDir);

        Url = entry.Url;
        Branch = entry.Branch;

        // Ensure the directory exists in the cache for cloning the repository.
        Directory = CacheManager.GetDirectoryInCache(cacheDirPath, new[] { "repositories", Hash });
    }

    public GitSource(string url, string? branch, string cacheDirPath, ICacheManager cacheManager)
    {
        CacheManager = cacheManager;
        CacheDir = new(cacheDirPath);
        CacheManager.Prepare(cacheDirPath);

        Url = url;
        Branch = branch;

        // Generate a unique hash for the repository based on its URL and branch.
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Url + Branch));
        var stringBuilder = new StringBuilder();
        foreach (var hashByte in hashBytes)
        {
            stringBuilder.Append(hashByte.ToString("x2"));
        }

        Hash = stringBuilder.ToString();

        // Ensure the directory exists in the cache for cloning the repository.
        Directory = CacheManager.GetDirectoryInCache(cacheDirPath, new[] { "repositories", Hash });

        // Store ID, URL, branch, and folder path in the cache DB, if it doesn't already exist
        using var db = new CacheContext(CacheDir);
        if (db.CachedGitSources.Find(Hash) != null)
        {
            return;
        }

        var entry = new CachedGitSource(Hash, Url, Branch, Directory.FullName);
        db.CachedGitSources.Add(entry);
        db.SaveChanges();
    }

    public string Hash { get; }
    private string Url { get; }
    private string? Branch { get; }
    public DirectoryInfo Directory { get; }

    private DirectoryInfo CacheDir { get; }

    private bool Cloned => Directory.GetFiles().Any() || Directory.GetDirectories().Any();

    private bool BranchDefined => !string.IsNullOrEmpty(Branch);

    private void Delete()
    {
        using var db = new CacheContext(CacheDir);
        var entry = db.CachedGitSources.Find(Hash);
        db.CachedGitSources.Remove(entry!);

        Directory.Delete(true);
    }

    private void Clone(string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"clone {Url} .", Directory.FullName);
        }
        catch (IOException e)
        {
            Delete();
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private void Checkout(string gitPath)
    {
        try
        {
            Invoke.Command(gitPath, $"checkout {Branch ?? ""}", Directory.FullName);
        }
        catch (IOException e)
        {
            Delete();
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
        }
    }

    private void Pull(string gitPath)
    {
        var branch = Branch;
        if (Branch == null)
        {
            branch = FetchCurrentBranch(gitPath);
        }

        string? commandOutput = null;

        try
        {
            commandOutput = Invoke.Command(gitPath, $"pull origin {branch ?? ""}", Directory.FullName)
                .Replace("\n", " ");
        }
        catch (IOException e)
        {
            if (commandOutput == "Already up to date.")
            {
                throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{e.Message}");
            }
        }
    }

    private string FetchCurrentBranch(string gitPath) =>
        Invoke.Command(gitPath, "branch --show-current", Directory.FullName).Replace("\n", "");

    public void CloneOrPull(string gitPath)
    {
        // If already cloned, pull instead.
        if (Cloned)
        {
            Pull(gitPath);
            return;
        }

        // If not yet cloned, clone from URL.
        Clone(gitPath);

        // If a branch is defined, checkout branch
        if (BranchDefined)
        {
            Checkout(gitPath);
        }
    }
}
