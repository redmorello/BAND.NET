using BAND.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BAND
{
    public class BandRequestException : BandException
    {
        public HttpResponseMessage Response { get; set; }

        public BandRequestException(HttpResponseMessage response, IEnumerable<ApiError> errors, string message = default(string), Exception innerEx = null)
            : base(message ?? $"Fitbit Request exception - Http Status Code: {response.StatusCode} - see errors for more details.", errors, innerEx)
        {
            this.Response = response;
        }
    }
}
