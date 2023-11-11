using System;

namespace Corgibytes.Freshli.Cli.Functionality.Api;

public class BomMetadataEntity
{
    public string OrganizationNickname { get; set; }
    public string ProjectNickname { get; set; }
    public RepositoryMetadataEntity Repository { get; set; }
    public DateTime DataPoint { get; set; }
    public CommitMetadataEntity Commit { get; set; }
    public ManifestMetadataEntity Manifest { get; set; }
}
