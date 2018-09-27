using System.Collections.Generic;
using Newtonsoft.Json;

namespace BAND.Models
{
    public class Bands
    {
        [JsonProperty("bands")]
        public List<Band> BandsList { get; set; }
    }
}
