using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Formatters
{
    public interface IOutputFormatter
    {
        FormatType Type { get; }
        string Format<T>(T entity);

        string Format<T>(IList<T> entities);
    }
}
