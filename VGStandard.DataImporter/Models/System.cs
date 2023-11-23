using Newtonsoft.Json;

namespace DataImporter.Models
{
    public class GameSystem
    {
        [JsonProperty("systemID")]
        public int SystemId { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("systemShortName")]
        public string SystemShortName { get; set; }

        [JsonProperty("systemHeaderSizeBytes")]
        public int? SystemHeaderSizeBytes { get; set; }

        [JsonProperty("systemHashless")]
        public string SystemHashless { get; set; }

        [JsonProperty("systemHeader")]
        public string SystemHeader { get; set; }

        [JsonProperty("systemSerial")]
        public string SystemSerial { get; set; }

        [JsonProperty("systemOEID")]
        public string SystemOEID { get; set; }
    }

}
