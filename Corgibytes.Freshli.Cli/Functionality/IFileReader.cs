namespace Corgibytes.Freshli.Cli.Functionality;

public interface IFileReader
{
    public JsonCycloneDx ToJson(string filePath);
}
