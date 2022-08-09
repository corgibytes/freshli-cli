using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.Commands.Cache;

public class PrepareCacheMessage
{
    public PrepareCacheMessage(string repositoryUrl, string? repositoryBranch, string historyInterval, string CacheDir)
    {
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        CacheDirectory = CacheDir;
    }

    [Required] public Guid Id { get; set; }
    [Required] public string RepositoryUrl { get; set; }

    public string? RepositoryBranch { get; set; }

    // TODO: Research how to use a value class here instead of a string
    [Required] public string HistoryInterval { get; set; }

    [Required] public string CacheDirectory { get; set; }
}
