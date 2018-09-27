using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BAND.OAuth2
{
    public class OAuth2Helper
    {
        private const string BandWebAuthBaseUrl = "https://auth.band.us";
        private const string BandApiBaseUrl = "https://openapi.band.us/v2";
        public static readonly string BandOauthPostUrl = "https://auth.band.us/oauth2/token";

        private const string OAuthBase = "/oauth2";

        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly string _redirectUri;

        public OAuth2Helper(BandAppCredentials credentials, string redirectUri)
        {
            _clientId = credentials.ClientId;
            _clientSecret = credentials.ClientSecret;
            _redirectUri = redirectUri;
        }

        public string GenerateAuthUrl(string[] scopeTypes, string state = null)
        {
            var sb = new StringBuilder();

            sb.Append(BandWebAuthBaseUrl);
            sb.Append(OAuthBase);
            sb.Append("/authorize?");
            sb.Append("response_type=code");
            sb.Append($"&client_id={_clientId}");
            sb.Append($"&redirect_uri={Uri.EscapeDataString(_redirectUri)}");
            //sb.Append(string.Format("&scope={0}", String.Join(" ", scopeTypes)));

            //if (!string.IsNullOrWhiteSpace(state))
            //    sb.Append(string.Format("&state={0}", state));

            return sb.ToString();
        }

        public async Task<OAuth2AccessToken> ExchangeAuthCodeForAccessTokenAsync(string code)
        {
            var url = $"{BandOauthPostUrl}?grant_type=authorization_code&code={code}";

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.3.0"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                string clientIdConcatSecret = Base64Encode(_clientId + ":" + _clientSecret);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", clientIdConcatSecret);

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    OAuth2AccessToken accessToken = ParseAccessTokenResponse(result);
                    //HttpResponseMessage resp = Request.CreateResponse(HttpStatusCode.OK, result);

                    return accessToken;
                }

                return new OAuth2AccessToken();
            }
        }

        public static OAuth2AccessToken ParseAccessTokenResponse(string responseString)
        {
            // assumption is the errors json will return in usual format eg. errors array
            JObject responseObject = JObject.Parse(responseString);

            var error = responseObject["errors"];
            if (error != null)
            {
                var errors = new JsonDotNetSerializer().ParseErrors(responseString);
                throw new BandException($"Unable to parse token response in method -- {nameof(ParseAccessTokenResponse)}.", errors);
            }

            var deserializer = new JsonDotNetSerializer();
            return deserializer.Deserialize<OAuth2AccessToken>(responseString);
        }

        /// <summary>
        /// Convert plain text to a base 64 encoded string - http://stackoverflow.com/a/11743162
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
