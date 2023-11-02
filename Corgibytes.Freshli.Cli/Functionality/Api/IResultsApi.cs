using System;
using System.Threading.Tasks;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Cache;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public interface IResultsApi
{
    string GetResultsUrl(Guid analysisId);
}
