using Newtonsoft.Json;

namespace BAND.Models
{
    public class Profile
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        [JsonProperty("user_key")]
        public string UserKey { get; set; }
        [JsonProperty("is_app_member")]
        public bool IsAppMember { get; set; }
        [JsonProperty("message_allowed")]
        public bool MessageAllowed { get; set; }
    }
}
