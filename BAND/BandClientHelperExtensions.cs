using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BAND
{
    internal static class BandClientHelperExtensions
    {
        /// <summary>
        /// Converts the REST api resource into the fully qualified url
        /// </summary>
        /// <param name="apiCall"></param>
        /// <param name="encodedUserId"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static string ToFullUrl(string apiCall, string encodedUserId = default(string), params object[] args)
        {
            string userSignifier = "-"; //used for current user
            if (!string.IsNullOrWhiteSpace(encodedUserId))
            {
                userSignifier = encodedUserId;
            }

            var parameters = new List<object> { userSignifier };
            if (args != null)
            {
                parameters.AddRange(args);
            }

            apiCall = string.Format(apiCall, parameters.ToArray());

            if (apiCall.StartsWith("/"))
            {
                apiCall = apiCall.TrimStart(new[] { '/' });
            }
            return Constants.BaseApiUrl + apiCall;
        }

        /// <summary>
        /// Converts the specified datetime value into the required format for calling the Fitbit API (yyyy-MM-dd)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        //internal static string ToFitbitFormat(this DateTime dateTime)
        //{
        //    const string DateFormat = "yyyy-MM-dd";
        //    return dateTime.ToString(DateFormat);
        //}

        /// <summary>
        /// Returns the elapsed seconds throughout a day; 00:00:00 to 23:59:59
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        internal static int ToElapsedSeconds(this DateTime dateTime)
        {
            int seconds = dateTime.Second;
            seconds += (dateTime.Minute * 60); // seconds in minutes
            seconds += (dateTime.Hour * 60 * 60); // seconds in hours
            return seconds;
        }

        /// <summary>
        /// Creates the processing request pipeline using the message handlers
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        internal static HttpMessageHandler CreatePipeline(this BandClient client, List<IBandInterceptor> interceptors)
        {
            if (interceptors.Count > 0)
            {
                // inspired by the code referenced from the web api source; this creates the russian doll effect
                BandHttpMessageHandler innerHandler = new BandHttpMessageHandler(client, interceptors[0]);

                var innerHandlers = interceptors.GetRange(1, interceptors.Count - 1);

                foreach (var handler in innerHandlers)
                {
                    var messageHandler = new BandHttpMessageHandler(client, handler);
                    messageHandler.InnerHandler = innerHandler;
                    innerHandler = messageHandler;
                }

                return innerHandler;
            }

            return null;
        }
    }
}
