using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivity : IApplicationTask
{
    public ValueTask Handle(IApplicationEventEngine eventClient);
}
