using BAND.Models;
using System;
using System.Collections.Generic;

namespace BAND
{
    public class BandException : Exception
    {
        public List<ApiError> ApiErrors { get; set; }

        public BandException(string message, IEnumerable<ApiError> errors, Exception innerEx = null) : base(message, innerEx)
        {
            ApiErrors = errors != null ? new List<ApiError>(errors) : new List<ApiError>();
        }
    }
}
