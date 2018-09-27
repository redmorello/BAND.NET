using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAND.Models
{
    public class Photos
    {
        [JsonProperty("paging")]
        public Paging Paging { get; set; }
        [JsonProperty("items")]
        public List<Photo> Items { get; set; }
    }
}
