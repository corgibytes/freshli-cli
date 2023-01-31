using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.Engine;

public interface IApplicationActivity
{
    public ValueTask Handle(IApplicationEventEngine eventClient);
}
