using Newtonsoft.Json;

namespace BAND.Models
{
    public class Band
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("band_key")]
        public string BandKey { get; set; }
        [JsonProperty("cover_image_url")]
        public string CoverImageUrl { get; set; }
        [JsonProperty("member_count")]
        public int MessageCount { get; set; }
        [JsonProperty("is_school_band")]
        public bool IsSchoolBand { get; set; }
    }
}
