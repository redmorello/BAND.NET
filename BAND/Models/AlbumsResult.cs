using Newtonsoft.Json;

namespace BAND.Models
{
    public class AlbumsResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public Albums ResultData { get; set; }
    }
}
