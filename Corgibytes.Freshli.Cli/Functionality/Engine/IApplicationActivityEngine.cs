using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivityEngine : IApplicationEngine
{
    public ValueTask Dispatch(IApplicationActivity applicationActivity);
}
