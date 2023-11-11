using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public interface IResultsApi
{
    ValueTask<PersonEntity?> GetPerson(CancellationToken cancellationToken);
    ValueTask UploadBomForManifest(CachedManifest manifest, string pathToBom, CancellationToken cancellationToken);
    ValueTask<IList<HistoryIntervalStop>> GetDataPoints(string repositoryHash, CancellationToken cancellationToken);
}
