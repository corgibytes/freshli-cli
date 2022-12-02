using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationEvent : IApplicationTask
{
    // ReSharper disable once UnusedParameter.Global
    public ValueTask Handle(IApplicationActivityEngine eventClient);
}
