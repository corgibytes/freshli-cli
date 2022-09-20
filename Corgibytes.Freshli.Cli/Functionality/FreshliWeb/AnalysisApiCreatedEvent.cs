using System;
using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.FreshliWeb;

public class AnalysisApiCreatedEvent : IApplicationEvent
{
    public Guid CachedAnalysisId { get; set; }
    public string Url { get; set; }
    public string Branch { get; set; }
    public string CacheDir { get; set; }
    public string GitPath { get; set; }
    public Guid ApiAnalysisId { get; set; }

    public void Handle(IApplicationActivityEngine eventClient) => throw new NotImplementedException();
}
