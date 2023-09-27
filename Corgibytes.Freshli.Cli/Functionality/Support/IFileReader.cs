using Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

namespace Corgibytes.Freshli.Cli.Functionality.Support;

public interface IFileReader
{
    public JsonCycloneDx ToJson(string filePath);
}
