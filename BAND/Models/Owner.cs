using Newtonsoft.Json;

namespace BAND.Models
{
    public class Owner
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("member_type")]
        public string MemberType { get; set; }
        [JsonProperty("member_certified")]
        public bool MemberCertified { get; set; }
        [JsonProperty("user_key")]
        public string UserKey { get; set; }
    }
}
