using Newtonsoft.Json;

namespace BAND.Models
{
    public class Paging
    {
        [JsonProperty("previous_params")]
        public object PreviousParams { get; set; }
        [JsonProperty("next_params")]
        public PagingObject NextParams { get; set; }
    }

    public class PagingObject
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("band_key")]
        public string BandKey { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("after")]
        public string After { get; set; }
    }
}