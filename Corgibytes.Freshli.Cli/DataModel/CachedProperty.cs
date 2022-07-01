using Microsoft.EntityFrameworkCore;

namespace Corgibytes.Freshli.Cli.DataModel;

[Index(nameof(Key), IsUnique = true)]
public class CachedProperty
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}
