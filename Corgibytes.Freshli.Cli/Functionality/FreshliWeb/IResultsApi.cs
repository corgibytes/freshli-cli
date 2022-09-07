using System;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public interface IResultsApi
{
    public string GetResultsUrl(Guid analysisId);
}
