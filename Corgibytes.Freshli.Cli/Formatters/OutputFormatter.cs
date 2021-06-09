using NLog;
using System;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli.Formatters
{
    public abstract class OutputFormatter : IOutputFormatter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public abstract FormatType Type { get; }

        public string Format<T>( T entity )
        {            
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            return this.Build<T>(entity);
        }

        public string Format<T>( IList<T> entities )
        {
            if(entities == null)
                throw new ArgumentNullException(nameof(entities));

            return this.Build<T>(entities);
        }

        protected abstract string Build<T>( T entity );

        protected abstract string Build<T>( IList<T> entities );

    }
}
