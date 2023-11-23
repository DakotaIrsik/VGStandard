using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VGStandard.Core.Metadata;

namespace VGStandard.DataImporter.Models;

[ElasticsearchType(IdProperty = nameof(Identifier))]
[Table("Region")]
public class Region : Trackable
{
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; } = Guid.NewGuid();

    [JsonProperty("regionName")]
    public string RegionName { get; set; } = string.Empty;
}
