using Newtonsoft.Json;

namespace BAND.Models
{
    public class ProfileResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public Profile ResultData { get; set; }
    }  
}
