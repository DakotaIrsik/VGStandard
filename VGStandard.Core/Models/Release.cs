using Nest;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VGStandard.Core.Metadata;

namespace VGStandard.DataImporter.Models;

[ElasticsearchType(IdProperty = nameof(Identifier))]
[Table("Release")]
public class Release : Trackable
{
    [JsonProperty("identifier")]
    public Guid Identifier { get; set; } = Guid.NewGuid();

    [JsonProperty("romid")]
    public int RomId { get; set; }

    [JsonProperty("rom")]
    public virtual Rom? Rom { get; set; } = null;

    [JsonProperty("releaseTitleName")]
    public string ReleaseTitleName { get; set; } = string.Empty;

    [JsonProperty("regionLocalizedID")]
    public int RegionLocalizedId { get; set; }

    [JsonProperty("TEMPregionLocalizedName")]
    public string TempRegionLocalizedName { get; set; } = string.Empty;

    [JsonProperty("TEMPsystemShortName")]
    public string TempSystemShortName { get; set; } = string.Empty;

    [JsonProperty("TEMPsystemName")]
    public string TempSystemName { get; set; } = string.Empty;

    [JsonProperty("releaseCoverFront")]
    public string? ReleaseCoverFront { get; set; } = string.Empty;

    [JsonProperty("releaseCoverBack")]
    public string? ReleaseCoverBack { get; set; } = string.Empty;

    [JsonProperty("releaseCoverCart")]
    public string? ReleaseCoverCart { get; set; } = string.Empty;

    [JsonProperty("releaseCoverDisc")]
    public string? ReleaseCoverDisc { get; set; } = string.Empty;

    [JsonProperty("releaseDescription")]
    public string? ReleaseDescription { get; set; } = string.Empty;

    [JsonProperty("releaseDeveloper")]
    public string? ReleaseDeveloper { get; set; } = string.Empty;

    [JsonProperty("releasePublisher")]
    public string? ReleasePublisher { get; set; } = string.Empty;

    [JsonProperty("releaseGenre")]
    public string? ReleaseGenre { get; set; } = string.Empty;

    [JsonProperty("releaseDate")]
    public string? ReleaseDate { get; set; } = string.Empty;  // Consider using DateTime? if the date is formatted

    [JsonProperty("releaseReferenceURL")]
    public string? ReleaseReferenceURL { get; set; } = string.Empty;

    [JsonProperty("releaseReferenceImageURL")]
    public string? ReleaseReferenceImageURL { get; set; } = string.Empty;
}
