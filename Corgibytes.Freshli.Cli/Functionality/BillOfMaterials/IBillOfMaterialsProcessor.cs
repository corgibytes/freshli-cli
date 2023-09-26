using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

public interface IBillOfMaterialsProcessor
{
    public Task AddLibYearMetadataDataToBom(
        CachedManifest manifest,
        // ReSharper disable once UnusedParameter.Global
        string agentExecutablePath,
        string pathToBom,
        CancellationToken cancellationToken);
}
