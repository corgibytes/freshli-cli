using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Corgibytes.Freshli.Cli.Functionality;

public class JsonCycloneDx
{
    public IList<Component> Components { get; set; }

    public static JsonCycloneDx FromJson(string json)
    {
        var options = new JsonSerializerOptions();
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter());

        return JsonSerializer.Deserialize<JsonCycloneDx>(json, options);
    }

    public class Component
    {
        public string Purl { get; set; }
    }
}

