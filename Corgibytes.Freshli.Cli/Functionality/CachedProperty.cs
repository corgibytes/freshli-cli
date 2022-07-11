using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedMember.Global

namespace Corgibytes.Freshli.Cli.Functionality;

[Index(nameof(Key), IsUnique = true)]
public class CachedProperty
{
    [Required] public int Id { get; set; }

    [Required] public string Key { get; set; } = null!;

    [Required] public string Value { get; set; } = null!;
}
