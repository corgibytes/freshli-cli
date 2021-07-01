using Castle.DynamicProxy;
using NLog;
using System;
using System.IO;
using System.Linq;

namespace Corgibytes.Freshli.Cli.IoC.Interceptors
{
    public class LoggerInterceptor : IInterceptor
    {
        private TextWriter Output { get; }
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public LoggerInterceptor( TextWriter output )
        {
            this.Output = output;
        }

        public void Intercept( IInvocation invocation )
        {
            try
            {
                string callingMessage = $"[{invocation.Method.Name} - Method Invocation Started ] - with parameters: {string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())}... ";
                string doneMessage = $"[{invocation.Method.Name} - Method Invocation Ended ] - returned:  {invocation.ReturnValue?.ToString()}.";

                this.Output.WriteLine(callingMessage);
                logger.Trace(callingMessage);

                invocation.Proceed();

                this.Output.WriteLine(doneMessage);
                logger.Trace(doneMessage);
            }
            catch(Exception e)
            {
                string message = $"[{invocation.Method.Name} - Unhandled Exception ] - {e.Message}";

                this.Output.WriteLine($"{message} - Take a look at the log for detailed information.");
                logger.Error($"{message} - {e.StackTrace}");
                throw;
            }
        }
    }
}
