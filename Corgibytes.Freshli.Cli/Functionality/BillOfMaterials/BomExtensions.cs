using System.Collections.Generic;
using CycloneDX.Models;

namespace Corgibytes.Freshli.Cli.Functionality.BillOfMaterials;

internal static class BomExtensions
{
    public static void AddMetadataProperties(this Bom bom, List<Property> properties)
    {
        bom.Metadata ??= new Metadata();
        bom.Metadata.Properties ??= new List<Property>();

        bom.Metadata.Properties.AddRange(properties);
    }
}
