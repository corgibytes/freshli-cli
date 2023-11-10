using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;
using Corgibytes.Freshli.Cli.Functionality.Git;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public interface IResultsApi
{
    ValueTask<Person?> GetPerson(CancellationToken cancellationToken);
    ValueTask UploadBomForManifest(CachedManifest manifest, string pathToBom);
    ValueTask<IList<HistoryIntervalStop>> GetDataPoints(string repositoryHash);
}
