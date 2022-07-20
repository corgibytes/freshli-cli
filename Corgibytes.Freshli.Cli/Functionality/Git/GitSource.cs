using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using CliWrap;
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

    public GitSource(string url, string? branch, DirectoryInfo cacheDir)
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
        Directory = Cache.GetDirectoryInCache(CacheDir, new[] { "repositories", Hash });

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
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{stdErrBuffer}");
        }
    }

    private void Checkout(string gitPath)
    {
        var stdErrBuffer = new StringBuilder();
        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("checkout")
                    .Add(Branch ?? "")
            )
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            Delete();
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{stdErrBuffer}");
        }
    }

    private void Pull(string gitPath)
    {
        var stdOutBuffer = new StringBuilder();
        var branch = Branch;
        if (Branch == null)
        {
            branch = FetchCurrentBranch(gitPath);
        }

        var stdErrBuffer = new StringBuilder();
        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("pull")
                    .Add("origin")
                    .Add(branch ?? "")
            )
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        var commandOutput = stdOutBuffer.ToString().Replace("\n", " ");

        if (task.Result.ExitCode != 0 && commandOutput.Equals("Already up to date.") == false)
        {
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{stdErrBuffer}");
        }
    }

    private string FetchCurrentBranch(string gitPath)
    {
        var stdErrBuffer = new StringBuilder();
        var stdOutBuffer = new StringBuilder();

        var command = CliWrap.Cli.Wrap(gitPath).WithArguments(
                args => args
                    .Add("branch")
                    .Add("--show-current")
            )
            .WithWorkingDirectory(Directory.FullName)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer));

        using var task = command.ExecuteAsync().Task;
        task.Wait();

        if (task.Result.ExitCode != 0)
        {
            throw new GitException($"{CliOutput.Exception_Git_EncounteredError}\n{stdErrBuffer}");
        }

        return stdOutBuffer.ToString().Replace("\n", "");
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
