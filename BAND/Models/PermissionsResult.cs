using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAND.Models
{
    public class PermissionsResult
    {
        [JsonProperty("result_code")]
        public int ResultCode { get; set; }

        [JsonProperty("result_data")]
        public BandPermissions ResultData { get; set; }
    }

    public class BandPermissions
    {
        public List<string> Permissions { get; set; }
    }
}
