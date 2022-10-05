using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using Corgibytes.Freshli.Cli.CommandOptions;
using Corgibytes.Freshli.Cli.Commands;
using Corgibytes.Freshli.Cli.Functionality.Doctor;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.CommandRunners;

public class DoctorRunner : CommandRunner<DoctorCommand, DoctorCommandOptions>
{
    private readonly IApplicationActivityEngine _activityEngine;

    public DoctorRunner(IServiceProvider serviceProvider, Runner runner, IApplicationActivityEngine activityEngine) : base(serviceProvider, runner)
    {
        _activityEngine = activityEngine;
    }

    public override int Run(DoctorCommandOptions options, InvocationContext context)
    {
        var startDoctorActivity = new StartDoctorActivity(options.GitPath, options.CacheDir, new List<Tuple<string, int>>());
        _activityEngine.Dispatch(startDoctorActivity);
        return startDoctorActivity.ErrorCode.Count != 0 ? 1 : 0;
    }
}
