using BAND.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace BAND
{
    public class BandTokenException : BandRequestException
    {
        public BandTokenException(HttpResponseMessage response, IEnumerable<ApiError> errors = null, string message = default(string))
            : base(response, errors, message ?? $"Fitbit Token exception - HTTP Status Code-- {response.StatusCode} -- see errors for more details.")
        {
        }
    }
}
