using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
[Table("CachedHistoryIntervalStops")]
public class CachedHistoryIntervalStop
{
    public CachedHistoryIntervalStop(string gitCommitId, string gitCommitDate)
    {
        GitCommitId = gitCommitId;
        GitCommitDate = gitCommitDate;
    }

    [Required] public int Id { get; set; }

    [Required] public string GitCommitDate { get; set; }

    [Required] public string GitCommitId { get; set; }

    [Required] public CachedAnalysis CachedAnalysis { get; set; }
}
