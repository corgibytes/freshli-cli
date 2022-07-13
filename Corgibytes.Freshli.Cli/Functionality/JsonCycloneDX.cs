using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.Functionality;

public class JsonCycloneDx
{
    public JsonCycloneDx(IList<Component> components) => Components = components;

    public IList<Component>? Components { get; set; }

    public static JsonCycloneDx? FromJson(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());

        return JsonSerializer.Deserialize<JsonCycloneDx>(json, options);
    }

    public class Component
    {
        public string? Purl { get; set; }
    }
}
