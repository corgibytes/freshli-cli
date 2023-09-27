using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.Functionality.Api.Auth;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Lib;

namespace Corgibytes.Freshli.Cli.Commands.Auth;

public class AuthCommandRunner : CommandRunner<AuthCommand, EmptyCommandOptions>
{
    private readonly ICacheManager _cacheManager;
    private readonly IAuthClient _authClient;

    public AuthCommandRunner(IServiceProvider serviceProvider, IRunner runner, ICacheManager cacheManager, IAuthClient authClient) : base(serviceProvider, runner)
    {
        _authClient = authClient;
        _cacheManager = cacheManager;
    }

    // TODO: Write a test for this method
    public override async ValueTask<int> Run(EmptyCommandOptions options, IConsole console, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _authClient.GetDevice(cancellationToken);
            console.WriteLine("Open this url in a browser to finish logging in:");
            console.WriteLine(token.LoginUrl);
            console.WriteLine("Ensure that the following code is displayed before attempting to log in:");
            console.WriteLine(token.UserCode);

            var credentials = await _authClient.GetCredentials(token, cancellationToken);
            var savedCredentialsPath = await _cacheManager.StoreApiCredentials(credentials);
            console.WriteLine($"Credentials saved in {savedCredentialsPath}");
            return 0;
        }
        catch (AuthException error)
        {
            console.WriteLine($"Authentication failed: {error.Reason}");
            console.WriteLine(error.Description);
            return -1;
        }
    }
}
