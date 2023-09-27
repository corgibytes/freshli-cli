using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Api.Auth;

public interface IAuthClient
{
    public Task<DeviceAuthToken> GetDevice(CancellationToken cancellationToken = default);
    public Task<ApiCredentials> GetCredentials(DeviceAuthToken token, CancellationToken cancellationToken = default);
}
