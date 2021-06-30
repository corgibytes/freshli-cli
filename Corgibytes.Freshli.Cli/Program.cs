
﻿using System;
using System.Globalization;
using System.IO;
using Corgibytes.Freshli.Lib;
﻿using Autofac;
using CommandLine;
using Corgibytes.Freshli.Cli.Factories;
using Corgibytes.Freshli.Cli.Options;

using NLog;
using System.Collections.Generic;

namespace Corgibytes.Freshli.Cli
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static ICommandRunnerFactory CommandRunnerFactory { get; set; }

        static Program()
        {
            CommandRunnerFactory = new IoCCommandRunnerFactory();
        }

        static void Main( string[] args )
        {       
            Parser.Default.ParseArguments<ScanOptions,AuthOptions>(args)
             .MapResult(
               ( ScanOptions opts ) => CommandRunnerFactory.CreateScanRunner(opts).Run(opts),
               ( AuthOptions opts ) => CommandRunnerFactory.CreateAuthRunner(opts).Run(opts),
               ( IEnumerable<Error> errs ) => ManageError(errs, args));
        }

        private static int ManageError( IEnumerable<Error> errors, string[] args )
        {
            logger.Error($"The following args can't be parsed - {string.Join(',', args)}, {errors}");
            return 1;
        }
    }
}
