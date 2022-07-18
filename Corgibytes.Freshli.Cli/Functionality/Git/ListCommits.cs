using System;
using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.Repositories;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public class ListCommits : IListCommits
{
    private readonly ICachedGitSourceRepository _cachedGitSourceRepository;

    public ListCommits(ICachedGitSourceRepository cachedGitSourceRepository) => _cachedGitSourceRepository = cachedGitSourceRepository;

    public IEnumerable<GitCommit> ForRepository(string repositoryId, DirectoryInfo cacheDirectory, string gitPath)
    {
        GitSource gitSource = new(repositoryId, cacheDirectory, _cachedGitSourceRepository);
        throw new NotImplementedException();
    }
}
