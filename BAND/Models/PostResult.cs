using Newtonsoft.Json;

namespace BAND.Models
{
    public class PostResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public ResultData ResultData { get; set; }
    }

    public class ResultData
    {
        public Post Post { get; set; }
    }
}
