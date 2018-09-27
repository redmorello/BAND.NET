using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAND.Models
{
    public class Comment
    {
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("author")]
        public Author Author { get; set; }
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
    }
}