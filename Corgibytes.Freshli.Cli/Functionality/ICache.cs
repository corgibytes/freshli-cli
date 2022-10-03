using System;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality;

public interface ICacheDb
{
    public Guid SaveAnalysis(CachedAnalysis analysis);
    public CachedAnalysis? RetrieveAnalysis(Guid id);
    public int AddHistoryIntervalStop(CachedHistoryIntervalStop historyIntervalStop);
    public void AddLibYear(CachedLibYear libYear);
}
