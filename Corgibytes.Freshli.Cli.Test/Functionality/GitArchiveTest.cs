using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Corgibytes.Freshli.Cli.Test.Common;
using Corgibytes.Freshli.Cli.Test.Repositories;
using Corgibytes.Freshli.Cli.Test.Services;
using Xunit;
using Xunit.Abstractions;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class GitArchiveTest : FreshliTest
{
    private readonly GitArchive _gitArchive;
    private readonly MockCachedGitSourceRepository _repository;

    public GitArchiveTest(ITestOutputHelper output) : base(output)
    {
        _repository = new();
        _gitArchive = new(_repository, new MockGitArchiveProcess());
    }

    [Fact]
    public void Verify_it_can_do_things()
    {
        var cachedGitSource = new CachedGitSource(
            "6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589",
            "http://any.url",
            "main",
            "tmp/.freshli"
        );
        _repository.addToList(cachedGitSource);

        Assert.Equal(
            "tmp/.freshli/histories/6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589/583d813db3e28b9b44a29db352e2f0e1b4c6e420",
            _gitArchive.CreateArchive(
                "6a2c5b97bc5113bda4845c09637043aef3d499f5b62eb252314a6bbcc7afd589",
                new("tmp/.freshli/histories"),
                new("583d813db3e28b9b44a29db352e2f0e1b4c6e420"),
                "git"
            )
        );
    }
}
