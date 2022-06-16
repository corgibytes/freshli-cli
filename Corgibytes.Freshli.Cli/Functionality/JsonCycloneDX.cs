using Newtonsoft.Json;

namespace Corgibytes.Freshli.Cli.Functionality;

public partial class JsonCycloneDx
{
    [JsonProperty("components")]
    public Component[] Components { get; set; }
}

public partial class Component
{
    [JsonProperty("purl")]
    public string Purl { get; set; }
}

public partial class JsonCycloneDx
{
    public static JsonCycloneDx FromJson(string json) => JsonConvert.DeserializeObject<JsonCycloneDx>(json);
}

