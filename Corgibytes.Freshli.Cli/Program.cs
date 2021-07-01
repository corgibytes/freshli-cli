using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Options;

namespace Corgibytes.Freshli.Cli
{
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static ICommandRunnerFactory CommandRunnerFactory { get; set; }

        static Program()
        {
            CommandRunnerFactory = new IoCCommandRunnerFactory();
        }

        static void Main( string[] args )
        {
            try
            {
                Parser.Default.ParseArguments<ScanOptions, AuthOptions>(args)
                 .MapResult(
                   ( ScanOptions opts ) => CommandRunnerFactory.CreateScanRunner(opts).Run(opts),
                   ( AuthOptions opts ) => CommandRunnerFactory.CreateAuthRunner(opts).Run(opts),
                   ( IEnumerable<Error> errs ) => ManageError(errs, args));
            }
            catch(Exception e)
            {
                Console.Out.WriteLine($"[Main - Unhandled Exception ] - {e.Message} - Take a look at the log for detailed information.");
            }
        }

        private static int ManageError( IEnumerable<Error> errors, string[] args )
        {
            logger.Error($"The following args can't be parsed - {string.Join(',', args)}, {errors}");
            return 1;
        }
    }
}
