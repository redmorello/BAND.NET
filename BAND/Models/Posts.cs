using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAND.Models
{
    public class Posts
    {
        [JsonProperty("paging")]
        public Paging Paging { get; set; }
        [JsonProperty("items")]
        public List<Post> Items { get; set; }
    }
}