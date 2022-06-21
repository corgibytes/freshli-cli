using System.Collections.Generic;
using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Formatters;

public class JsonOutputFormatter : OutputFormatter
{
    public override FormatType Type => FormatType.Json;

    protected override string Build<T>(T entity)
    {
        return JsonConvert.SerializeObject(entity, Formatting.Indented);
    }

    protected override string Build<T>(IList<T> entities)
    {
        return JsonConvert.SerializeObject(entities, Formatting.Indented);
    }
}
