using System;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb, IDisposable
{
    private bool _disposed;

    public CacheDb(string cacheDir)
    {
        CacheDir = cacheDir;
        Db = new CacheContext(cacheDir);
    }

    private CacheContext Db { get; }
    public string CacheDir { get; }

    public Guid SaveAnalysis(CachedAnalysis analysis)
    {
        var savedEntity = Db.CachedAnalyses.Add(analysis);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedAnalysis? RetrieveAnalysis(Guid id) => Db.CachedAnalyses.Find(id);

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
