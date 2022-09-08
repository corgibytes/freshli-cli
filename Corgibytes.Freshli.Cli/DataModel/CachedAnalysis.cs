using System;
using System.ComponentModel.DataAnnotations;
using Corgibytes.Freshli.Cli.Functionality;
using Microsoft.EntityFrameworkCore;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
public class CachedAnalysis
{
    public CachedAnalysis(string repositoryUrl, string? repositoryBranch, string historyInterval, CommitHistory useCommitHistory)
    {
        RepositoryUrl = repositoryUrl;
        RepositoryBranch = repositoryBranch;
        HistoryInterval = historyInterval;
        UseCommitHistory = useCommitHistory;
    }

    [Required] public Guid Id { get; set; }
    [Required] public string RepositoryUrl { get; set; }

    public string? RepositoryBranch { get; set; }
    public CommitHistory UseCommitHistory { get; set; }

    // TODO: Research how to use a value class here instead of a string
    [Required] public string HistoryInterval { get; set; }
}
