using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAND.Models
{
    public class Albums
    {
        [JsonProperty("paging")]
        public Paging Paging { get; set; }
        [JsonProperty("items")]
        public List<Album> Items { get; set; }
        [JsonProperty("total_photo_count")]
        public int TotalPhotoCount { get; set; }
    }
}
