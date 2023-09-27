using System;
using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;
using Corgibytes.Freshli.Cli.Functionality.Support;

namespace Corgibytes.Freshli.Cli.Test.Functionality.BillOfMaterials;

public class MockFileReader : IFileReader
{
    private string _jsonString = "";

    public JsonCycloneDx ToJson(string filePath) =>
        JsonCycloneDx.FromJson(_jsonString) ?? throw new InvalidOperationException();

    public void FeedJson(string json) => _jsonString = json;
}
