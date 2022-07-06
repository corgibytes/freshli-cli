using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Formatters;

public abstract class OutputFormatter : IOutputFormatter
{
    public abstract FormatType Type { get; }

    public virtual string Format<T>(T entity)
    {
        return Build<T>(entity ?? throw new ArgumentNullException(nameof(entity)));
    }

    public virtual string Format<T>(IList<T> entities)
    {
        return Build<T>(entities ?? throw new ArgumentNullException(nameof(entities)));
    }

    protected abstract string Build<T>(T entity);

    protected abstract string Build<T>(IList<T> entities);

}
