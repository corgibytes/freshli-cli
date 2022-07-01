using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality;
using Corgibytes.Freshli.Cli.Repositories;

namespace Corgibytes.Freshli.Cli.Test.Repositories;

public class MockCachedGitSourceRepository : ICachedGitSourceRepository
{
    private List<CachedGitSource> _list;

    public MockCachedGitSourceRepository() => _list = new();

    public void addToList(CachedGitSource cachedGitSource)
    {
        _list.Add(cachedGitSource);
    }

    public CachedGitSource FindOneByHash(string hash, DirectoryInfo cacheDir)
    {
        return _list.Find(gitSource => gitSource.Id == hash);
    }
}

