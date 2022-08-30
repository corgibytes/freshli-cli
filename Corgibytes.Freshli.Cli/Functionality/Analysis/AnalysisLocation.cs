namespace Corgibytes.Freshli.Cli.Functionality.Analysis;

public class AnalysisLocation : IAnalysisLocation
{
    public AnalysisLocation(string path) => Path = path;

    public string Path { get; }
}
