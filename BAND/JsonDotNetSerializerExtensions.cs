using BAND.Models;
using System;
using System.Collections.Generic;

namespace BAND
{
    internal static class JsonDotNetSerializerExtensions
    {
        /// <summary>
        /// Parses the error structure which is common when errors are raised from the api
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="errorJson"></param>
        /// <returns></returns>
        internal static List<ApiError> ParseErrors(this JsonDotNetSerializer serializer, string errorJson)
        {
            if (string.IsNullOrWhiteSpace(errorJson))
            {
                throw new ArgumentNullException(nameof(errorJson), "errorJson can not be empty, null or whitespace");
            }

            serializer.RootProperty = "errors";
            return serializer.Deserialize<List<ApiError>>(errorJson);
        }
    }
}
