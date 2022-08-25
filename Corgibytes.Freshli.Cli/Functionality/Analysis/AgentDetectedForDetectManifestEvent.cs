using Corgibytes.Freshli.Cli.Functionality.Engine;

namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AgentDetectedForDetectManifestEvent : IApplicationEvent
{
    public string RepositoryId { get; init; } = null!;
    public string CommitId { get; init; } = null!;
    public string AgentPath { get; init; } = null!;

    public void Handle(IApplicationActivityEngine eventClient) => throw new System.NotImplementedException();
}
