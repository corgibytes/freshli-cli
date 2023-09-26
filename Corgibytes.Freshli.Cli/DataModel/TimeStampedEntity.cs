using System;

namespace Corgibytes.Freshli.Cli.DataModel;

public abstract class TimeStampedEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
