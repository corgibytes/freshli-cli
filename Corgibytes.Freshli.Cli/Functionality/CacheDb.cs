using System;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;

namespace Corgibytes.Freshli.Cli.Functionality;

public class CacheDb : ICacheDb, IDisposable
{
    private bool _disposed;

    public CacheDb(string cacheDir)
    {
        var cacheDirectory = new DirectoryInfo(cacheDir);
        Db = new(cacheDirectory);
    }

    private CacheContext Db { get; }

    public Guid SaveAnalysis(CachedAnalysis analysis)
    {
        var savedEntity = Db.CachedAnalyses.Add(analysis);
        Db.SaveChanges();
        return savedEntity.Entity.Id;
    }

    public CachedAnalysis? RetrieveAnalysis(Guid id)
    {
        return Db.CachedAnalyses.Find(id);
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
