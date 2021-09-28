using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Formatters
{   
    public abstract class OutputFormatter : IOutputFormatter
    {
        public abstract FormatType Type { get; }

        public virtual string Format<T>(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Build<T>(entity);
        }

        public virtual string Format<T>(IList<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Build<T>(entities);
        }

        protected abstract string Build<T>(T entity);

        protected abstract string Build<T>(IList<T> entities);

    }
}
