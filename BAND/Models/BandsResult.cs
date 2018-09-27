using Newtonsoft.Json;

namespace BAND.Models
{
    public class BandsResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public Bands ResultData { get; set; }
    }  
}
