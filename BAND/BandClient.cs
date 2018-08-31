using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace BAND
{
    public class BandClient : IBandClient
    {


        /// <summary>
        /// The httpclient which will be used for the api calls through the FitbitClient instance
        /// </summary>
        public HttpClient HttpClient { get; private set; }
    }
}
