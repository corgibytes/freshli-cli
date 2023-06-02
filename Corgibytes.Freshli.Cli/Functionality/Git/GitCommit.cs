using System;

namespace Corgibytes.Freshli.Cli.Functionality.Git;

public record GitCommit(string ShaIdentifier, DateTimeOffset CommittedAt);
