using Corgibytes.Freshli.Cli.Functionality;

namespace Corgibytes.Freshli.Cli.Test.Functionality;

public class MockReadFile : IReadFile
{
    private string _jsonString;

    public void FeedJson(string json)
    {
        _jsonString = json;
    }

    public JsonCycloneDx ToJson(string filePath)
    {
        return JsonCycloneDx.FromJson(_jsonString);
    }
}
