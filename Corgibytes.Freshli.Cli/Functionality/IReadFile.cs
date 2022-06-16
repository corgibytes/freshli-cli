namespace Corgibytes.Freshli.Cli.Functionality;

public interface IReadFile
{
    public JsonCycloneDx ToJson(string filePath);
}
