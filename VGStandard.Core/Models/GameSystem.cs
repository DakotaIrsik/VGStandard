using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VGStandard.Core.Metadata;

namespace VGStandard.DataImporter.Models;

[ElasticsearchType(IdProperty = nameof(Identifier))]
[Table("System")] //this part
public class GameSystem : Trackable
{

    [JsonProperty("identifier")]
    public Guid Identifier { get; set; } = Guid.NewGuid();

    [JsonProperty("systemName")]
    public string SystemName { get; set; } = string.Empty;

    [JsonProperty("systemShortName")]
    public string SystemShortName { get; set; } = string.Empty;

    [JsonProperty("systemHeaderSizeBytes")]
    public int? SystemHeaderSizeBytes { get; set; }

    [JsonProperty("systemHashless")]
    public string? SystemHashless { get; set; } = string.Empty;

    [JsonProperty("systemHeader")]
    public string? SystemHeader { get; set; } = string.Empty;

    [JsonProperty("systemSerial")]
    public string? SystemSerial { get; set; } = string.Empty;

    [JsonProperty("systemOEID")]
    public string SystemOEID { get; set; } = string.Empty;
}
