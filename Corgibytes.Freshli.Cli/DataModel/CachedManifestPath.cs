using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Id), IsUnique = true)]
[Index(nameof(HistoryStopPointId))]
[Index(nameof(HistoryStopPointId), nameof(AgentExecutablePath))]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CachedManifestPath
{
    [Required] public int Id { get; set; }
    [Required] public string AgentExecutablePath { get; set; } = null!;
    [Required] public string ManifestPath { get; set; } = null!;

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    [Required] public int HistoryStopPointId { get; set; }
    public virtual CachedHistoryStopPoint HistoryStopPoint { get; set; } = null!;
}
