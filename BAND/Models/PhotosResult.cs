using Newtonsoft.Json;

namespace BAND.Models
{
    public class PhotosResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public Photos ResultData { get; set; }
    }
}
