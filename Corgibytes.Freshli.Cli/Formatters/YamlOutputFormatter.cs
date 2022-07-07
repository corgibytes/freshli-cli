using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Corgibytes.Freshli.Cli.Formatters;

public class YamlOutputFormatter : OutputFormatter
{
    public override FormatType Type => FormatType.Yaml;

    protected override string Build<T>(T entity) => new Serializer().Serialize(entity);

    protected override string Build<T>(IList<T> entities) => new Serializer().Serialize(entities);
}
