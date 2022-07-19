using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using CliWrap;

namespace Corgibytes.Freshli.Cli.Functionality;

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

public class GitRepository
{
    // ReSharper disable once UnusedMember.Global
    public GitRepository(string hash, DirectoryInfo cacheDir)
    {
        // Ensure the cache directory is ready for use.
        CacheDir = cacheDir;
        Cache.Prepare(CacheDir);

        Hash = hash;

        // Get existing entry via provided hash
        using var db = new CacheContext(CacheDir);
        var entry = db.CachedGitRepos.Find(Hash);
        if (entry == null)
        {
            throw new CacheException("No repository with this hash exists in cache.");
        }

        Url = entry.Url;
        Branch = entry.Branch;

        // Ensure the directory exists in the cache for cloning the repository.
        Directory = Cache.GetDirectoryInCache(CacheDir, new[] { "repositories", Hash });
    }

    public GitRepository(string url, string branch, DirectoryInfo cacheDir, string directory ="")
    {
        // Ensure the cache directory is ready for use.
        CacheDir = cacheDir;
        Cache.Prepare(CacheDir);

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
        Directory = Cache.GetDirectoryInCache(CacheDir, new[] { directory.Length == 0 ? "repositories" : directory, Hash });

        // Store ID, URL, branch, and folder path in the cache DB, if it doesn't already exist
        using var db = new CacheContext(CacheDir);
        if (db.CachedGitRepos.Find(Hash) != null)
        {
            return;
        }

        var entry = new CachedGitRepo
        {
            Id = Hash,
            Url = Url,
            Branch = Branch,
            LocalPath = Directory.FullName
        };
        db.CachedGitRepos.Add(entry);
        db.SaveChanges();
    }

    public string Hash { get; }
    private string Url { get; }
    private string Branch { get; }
    private DirectoryInfo Directory { get; }

    private DirectoryInfo CacheDir { get; }

    private bool Cloned => Directory.GetFiles().Any() || Directory.GetDirectories().Any();

    private bool BranchDefined => !string.IsNullOrEmpty(Branch);

    private void Delete()
    {
        using var db = new CacheContext(CacheDir);
        var entry = db.CachedGitRepos.Find(Hash);
        db.CachedGitRepos.Remove(entry!);

        Directory.Delete(true);
    }

    private void Clone(string gitPath)
    {
        var stdErrBuffer = new StringBuilder();
        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("clone")
                    .Add(Url)
                    .Add('.')
            )
            .WithValidation(CommandResultValidation.None)
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            Delete();
            throw new GitException($"Git encountered an error:\n{stdErrBuffer}");
        }
    }

    private void Checkout(string gitPath)
    {
        var stdErrBuffer = new StringBuilder();
        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("checkout")
                    .Add(Branch)
            )
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            Delete();
            throw new GitException($"Git encountered an error:\n{stdErrBuffer}");
        }
    }

    private void Pull(string gitPath)
    {
        var stdErrBuffer = new StringBuilder();

        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("pull")
                    .Add("origin")
                    .Add(Branch)
            )
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            throw new GitException($"Git encountered an error:\n{stdErrBuffer}");
        }
    }

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
