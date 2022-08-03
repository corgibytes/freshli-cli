using Hangfire;

namespace Corgibytes.Freshli.Cli.IoC.Engine;

// from https://github.com/HangfireIO/Hangfire/blob/c63127851a8f8a406f22fd14ae3e94d3124e9e8a/src/Hangfire.AspNetCore/IRecurringJobManagerFactory.cs#L18
// This class has been modified from the original version to match the conventions of this project
public interface IRecurringJobManagerFactory
{
    IRecurringJobManager GetManager(JobStorage storage);
}
