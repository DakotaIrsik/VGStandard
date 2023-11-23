using Nest;
using Newtonsoft.Json;

namespace DataImporter.Models
{
    public class Rom
    {
        [JsonProperty("romID")]
        public int RomId { get; set; }

        [JsonProperty("systemID")]
        public int SystemId { get; set; }

        [JsonProperty("regionID")]
        public int RegionId { get; set; }

        [JsonProperty("romHashCRC")]
        public string RomHashCRC { get; set; }

        [JsonProperty("romHashMD5")]
        public string RomHashMD5 { get; set; }

        [JsonProperty("romHashSHA1")]
        public string RomHashSHA1 { get; set; }

        [JsonProperty("romSize")]
        public long? RomSize { get; set; }

        [JsonProperty("romFileName")]
        public string RomFileName { get; set; }

        [JsonProperty("romExtensionlessFileName")]
        public string RomExtensionlessFileName { get; set; }

        [JsonProperty("romParent")]
        public string RomParent { get; set; }

        [JsonProperty("romSerial")]
        public string RomSerial { get; set; }

        [JsonProperty("romHeader")]
        public string RomHeader { get; set; }

        [JsonProperty("romLanguage")]
        public string RomLanguage { get; set; }

        [JsonProperty("TEMPromRegion")]
        public string TempRomRegion { get; set; }

        [JsonProperty("romDumpSource")]
        public string RomDumpSource { get; set; }
    }

}
