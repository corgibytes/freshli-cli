using System;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb, IDisposable
{
    private bool _disposed;

    public CacheDb(string cacheDir) => Db = new CacheContext(cacheDir);

    private CacheContext Db { get; }

    public Guid SaveAnalysis(CachedAnalysis analysis)
    {
        var savedEntity = Db.CachedAnalyses.Add(analysis);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedAnalysis? RetrieveAnalysis(Guid id) => Db.CachedAnalyses.Find(id);

    public void AddHistoryIntervalStop(Guid analysisId, CachedHistoryIntervalStop historyIntervalStop)
    {
        var cachedAnalysis = Db.CachedAnalyses.Find(analysisId);
        cachedAnalysis?.HistoryIntervalStops.Add(historyIntervalStop);
        Db.SaveChanges();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Db.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
