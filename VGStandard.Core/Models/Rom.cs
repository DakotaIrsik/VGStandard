using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VGStandard.Core.Metadata;

namespace VGStandard.DataImporter.Models;

[ElasticsearchType(IdProperty = nameof(Identifier))]
[Table("Rom")]
public class Rom : Trackable
{
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; } = Guid.NewGuid();

    [JsonProperty("systemID")]
    public int SystemId { get; set; }

    [JsonProperty("regionID")]
    public int RegionId { get; set; }

    [JsonProperty("romHashCRC")]
    public string? RomHashCRC { get; set; } = string.Empty;

    [JsonProperty("romHashMD5")]
    public string? RomHashMD5 { get; set; } = string.Empty;

    [JsonProperty("romHashSHA1")]
    public string? RomHashSHA1 { get; set; } = string.Empty;

    [JsonProperty("romSize")]
    public long? RomSize { get; set; }

    [JsonProperty("romFileName")]
    public string RomFileName { get; set; } = string.Empty;

    [JsonProperty("romExtensionlessFileName")]
    public string RomExtensionlessFileName { get; set; } = string.Empty;

    [JsonProperty("romParent")]
    public string? RomParent { get; set; } = string.Empty;

    [JsonProperty("romSerial")]
    public string? RomSerial { get; set; } = string.Empty;

    [JsonProperty("romHeader")]
    public string? RomHeader { get; set; } = string.Empty;

    [JsonProperty("romLanguage")]
    public string? RomLanguage { get; set; } = string.Empty;

    [JsonProperty("TEMPromRegion")]
    public string? TempRomRegion { get; set; } = string.Empty;

    [JsonProperty("romDumpSource")]
    public string? RomDumpSource { get; set; } = string.Empty;
}
