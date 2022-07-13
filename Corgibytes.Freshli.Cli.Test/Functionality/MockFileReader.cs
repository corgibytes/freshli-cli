using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class MockFileReader : IFileReader
{
    private string _jsonString;

    public JsonCycloneDx ToJson(string filePath) => JsonCycloneDx.FromJson(_jsonString);

    public void FeedJson(string json) => _jsonString = json;
}
