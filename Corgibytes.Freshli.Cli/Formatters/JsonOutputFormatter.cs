using System.Collections.Generic;
using System.Text.Json;

namespace Corgibytes.Freshli.Cli.Formatters;

public class JsonOutputFormatter : OutputFormatter
{
    public override FormatType Type => FormatType.Json;

    protected override string Build<T>(T entity) =>
        JsonSerializer.Serialize(entity, new JsonSerializerOptions { WriteIndented = true });

    protected override string Build<T>(IList<T> entities) =>
        JsonSerializer.Serialize(entities, new JsonSerializerOptions { WriteIndented = true });
}
