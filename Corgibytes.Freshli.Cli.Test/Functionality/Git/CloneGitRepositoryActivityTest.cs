using Corgibytes.Freshli.Cli.DataModel;
using Corgibytes.Freshli.Cli.Exceptions;
using Corgibytes.Freshli.Cli.Functionality.Engine;
using Corgibytes.Freshli.Cli.Functionality.Git;
using Moq;
using Xunit;

namespace Corgibytes.Freshli.Cli.Test.Functionality.Git;

[UnitTest]
public class CloneGitRepositoryActivityTest
{
    private readonly string _branch;

    private readonly string _cacheDir;
    private readonly Mock<IApplicationEventEngine> _eventEngine = new();
    private readonly string _gitPath;
    private readonly Mock<ICachedGitSourceRepository> _gitSourceRepository = new();
    private readonly string _localPath;
    private readonly string _repositoryId;
    private readonly string _url;

    public CloneGitRepositoryActivityTest()
    {
        _cacheDir = "example";
        _url = "http://git.exaple.com";
        _branch = "main";

        _gitPath = "git";
        _repositoryId = "test";
        _localPath = "test";
    }

    [Fact]
    public void HandlerFiresGitRepositoryClonedEventWhenInvokedFromCli()
    {
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(_url, _branch, _cacheDir, _gitPath))
            .Returns(new CachedGitSource(_repositoryId, _url, _branch, _localPath));

        var activity = new CloneGitRepositoryActivity(_gitSourceRepository.Object, _url, _branch, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<GitRepositoryClonedEvent>(value => value.GitRepositoryId == _repositoryId)));
    }

    [Fact]
    public void HandlerFiresCloneGitRepositoryFailedEventWhenCloneFailsAndInvokedFromCli()
    {
        _gitSourceRepository.Setup(mock => mock.CloneOrPull(_url, _branch, _cacheDir, _gitPath))
            .Throws(new GitException("Git clone failed"));

        var activity = new CloneGitRepositoryActivity(_gitSourceRepository.Object, _url, _branch, _cacheDir, _gitPath);

        activity.Handle(_eventEngine.Object);

        _eventEngine.Verify(mock =>
            mock.Fire(It.Is<CloneGitRepositoryFailedEvent>(value => value.ErrorMessage == "Git clone failed")));
    }
}
