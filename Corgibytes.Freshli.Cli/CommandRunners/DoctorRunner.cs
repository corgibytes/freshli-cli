using System;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Doctor;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class DoctorRunner : CommandRunner<DoctorCommand, DoctorCommandOptions>
{
    private readonly IApplicationActivityEngine _activityEngine;
    private readonly IApplicationEventEngine _eventEngine;

    public DoctorRunner(IServiceProvider serviceProvider, Runner runner, IApplicationActivityEngine activityEngine,
        IApplicationEventEngine eventEngine) : base(serviceProvider, runner)
    {
        _activityEngine = activityEngine;
        _eventEngine = eventEngine;
    }

    public override int Run(DoctorCommandOptions options, InvocationContext context)
    {
        _activityEngine.Dispatch(new StartDoctorActivity(options.GitPath, options.CacheDir));
        return 0;
    }
}
