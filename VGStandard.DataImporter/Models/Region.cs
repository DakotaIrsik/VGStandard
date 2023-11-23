using Newtonsoft.Json;

namespace DataImporter.Models;

public class Region
{
    [JsonProperty("regionId")]
    public int RegionId { get; set; }

    [JsonProperty("regionName")]
    public string RegionName { get; set; }
}
