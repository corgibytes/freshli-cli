using System.Threading;
using System.Threading.Tasks;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public interface IBillOfMaterialsProcessor
{
    public Task AddLibYearMetadataDataToBom(string agentExecutablePath, string pathToBom,
        CancellationToken cancellationToken);
}
