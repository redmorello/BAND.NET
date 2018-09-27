using Newtonsoft.Json;

namespace BAND.Models
{
    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        [JsonProperty("user_key")]
        public string UserKey { get; set; }
    }
}