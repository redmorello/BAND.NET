using Newtonsoft.Json;

namespace BAND.Models
{
    public class Album
    {
        [JsonProperty("latest_photos")]
        public object LatestPhotos { get; set; }
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
        [JsonProperty("photo_count")]
        public int PhotoCount { get; set; }
        [JsonProperty("photo_album_key")]
        public string PhotoAlbumKey { get; set; }
        [JsonProperty("owner")]
        public Owner Owner { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string BandKey { get; set; }
    }
}
