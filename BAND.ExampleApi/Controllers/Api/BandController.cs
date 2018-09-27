using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using BAND.Models;
using BAND.OAuth2;
using Newtonsoft.Json;

namespace BAND.ExampleApi.Controllers.Api
{
    [Route("api/band")]
    public class BandController : ApiController
    {
        private BandAppCredentials _credentials;
        private const string BandApiBaseUrl = "https://openapi.band.us/v2";

        [Route("api/band/authorize")]
        [HttpGet]
        public HttpResponseMessage Authorize()
        {
            var appCredentials = new BandAppCredentials()
            {
                ClientId = ConfigurationManager.AppSettings["BandClientId"],
                ClientSecret = ConfigurationManager.AppSettings["BandClientSecret"]
            };

            _credentials = appCredentials;

            //Provide the App Credentials. You get those by registering your app at dev.fitbit.com
            //Configure Fitbit authenticaiton request to perform a callback to this constructor's Callback method
            //var authenticator = new OAuth2Helper(appCredentials, Request.Url.GetLeftPart(UriPartial.Authority) + "/band/callback");
            var authenticator = new OAuth2Helper(appCredentials, ConfigurationManager.AppSettings["BaseUrl"] + "/api/band/callback");
            string[] scopes = new string[] { "profile" };

            string authUrl = authenticator.GenerateAuthUrl(scopes, null);

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.3.0"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                
                HttpResponseMessage response = client.GetAsync(authUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                return response;
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        
        [Route("api/band/callback")]
        [HttpGet]
        public async Task<HttpResponseMessage> Callback(string code)
        {
            var appCredentials = new BandAppCredentials()
            {
                ClientId = ConfigurationManager.AppSettings["BandClientId"],
                ClientSecret = ConfigurationManager.AppSettings["BandClientSecret"]
            };

            var authenticator = new OAuth2Helper(appCredentials, ConfigurationManager.AppSettings["BaseUrl"] + "/api/Band/Callback");

            OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            if (!string.IsNullOrEmpty(accessToken.Token))
            {
                //Store credentials in FitbitClient. The client in its default implementation manages the Refresh process
                //var fitbitClient = GetBandClient(accessToken);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(JsonConvert.SerializeObject(accessToken), Encoding.UTF8);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        [Route("api/band/profile")]
        [HttpGet]
        public async Task<HttpResponseMessage> Profile()
        {
            var token = "ZQAAAZWi3setRi_kjYf6GL6IqzFrw8BYViPIK93xdAlIx9g4Jk294IHMalJVZsiseBozH4ZNYnWpllA__8Qie2M_Cj9ORX9wIs_zH7Jt6H_6P7t-";
            var url = $"{BandApiBaseUrl}/profile?access_token={token}";

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.3.0"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<ProfileResult>().Result;
                    HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK, result);

                    return resp;
                }

                return response;
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
    }
}
