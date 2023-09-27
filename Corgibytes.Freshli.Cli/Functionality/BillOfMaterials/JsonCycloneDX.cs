using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

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
        public List<ComponentHash>? Hashes { get; set; }
    }

    public class ComponentHash
    {
        // ReSharper disable once UnusedMember.Global
        public string? Alg { get; set; }

        // ReSharper disable once UnusedMember.Global
        public string? Content { get; set; }
    }
}
