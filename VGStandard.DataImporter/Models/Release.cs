using Newtonsoft.Json;

namespace DataImporter.Models;

public class Release
{
    [JsonProperty("releaseID")]
    public int ReleaseId { get; set; }

    [JsonProperty("romID")]
    public int RomId { get; set; }

    [JsonProperty("releaseTitleName")]
    public string ReleaseTitleName { get; set; }

    [JsonProperty("regionLocalizedID")]
    public int RegionLocalizedId { get; set; }

    [JsonProperty("TEMPregionLocalizedName")]
    public string TempRegionLocalizedName { get; set; }

    [JsonProperty("TEMPsystemShortName")]
    public string TempSystemShortName { get; set; }

    [JsonProperty("TEMPsystemName")]
    public string TempSystemName { get; set; }

    [JsonProperty("releaseCoverFront")]
    public string ReleaseCoverFront { get; set; }

    [JsonProperty("releaseCoverBack")]
    public string ReleaseCoverBack { get; set; }

    [JsonProperty("releaseCoverCart")]
    public string ReleaseCoverCart { get; set; }

    [JsonProperty("releaseCoverDisc")]
    public string ReleaseCoverDisc { get; set; }

    [JsonProperty("releaseDescription")]
    public string ReleaseDescription { get; set; }

    [JsonProperty("releaseDeveloper")]
    public string ReleaseDeveloper { get; set; }

    [JsonProperty("releasePublisher")]
    public string ReleasePublisher { get; set; }

    [JsonProperty("releaseGenre")]
    public string ReleaseGenre { get; set; }

    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }  // Consider using DateTime? if the date is formatted

    [JsonProperty("releaseReferenceURL")]
    public string ReleaseReferenceURL { get; set; }

    [JsonProperty("releaseReferenceImageURL")]
    public string ReleaseReferenceImageURL { get; set; }
}
