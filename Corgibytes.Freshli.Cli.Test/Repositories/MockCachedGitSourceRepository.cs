using System.Collections.Generic;
using System.IO;
using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Repositories;

namespace Corgibytes.Freshli.Cli.Test.Repositories;

public class MockCachedGitSourceRepository : ICachedGitSourceRepository
{
    private readonly List<CachedGitSource> _list;

    public MockCachedGitSourceRepository() => _list = new();

    public CachedGitSource FindOneByHash(string hash, DirectoryInfo cacheDir) =>
        _list.Find(gitSource => gitSource.Id == hash);

    public void addToList(CachedGitSource cachedGitSource) => _list.Add(cachedGitSource);
}
