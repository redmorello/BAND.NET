using Newtonsoft.Json;

namespace BAND.Models
{
    public class PostsResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public Posts ResultData { get; set; }
    }
}
