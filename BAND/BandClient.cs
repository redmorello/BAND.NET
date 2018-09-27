using System;
using System.Collections.Generic;
using System.Net;
using BAND.OAuth2;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BAND.Models;

namespace BAND
{
    public class BandClient : IBandClient
    {
        public BandAppCredentials AppCredentials { get; private set; }

        private OAuth2AccessToken _accessToken;
        public OAuth2AccessToken AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
                //If we update the AccessToken after HttpClient has been created, then reconfigure authorization header
                if (HttpClient != null)
                    ConfigureAuthorizationHeader();
            }
        }

        /// <summary>
        /// The httpclient which will be used for the api calls through the FitbitClient instance
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        public ITokenManager TokenManager { get; private set; }
        public bool OAuth2TokenAutoRefresh { get; set; }
        public List<IBandInterceptor> BandInterceptorPipeline { get; private set; }

        /// <summary>
        /// Simplest constructor for OAuth2- requires the minimum information required by FitBit.Net client to make succesful calls to Fitbit Api
        /// </summary>
        /// <param name="credentials">Obtain this information from your developer dashboard. App credentials are required to perform token refresh</param>
        /// <param name="accessToken">Authenticate with Fitbit API using OAuth2. Authenticator2 class is a helper for this process</param>
        /// <param name="interceptor">An interface that enables sniffing all outgoing and incoming http requests from FitbitClient</param>
        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, IBandInterceptor interceptor = null, bool enableOAuth2TokenRefresh = true, ITokenManager tokenManager = null)
        {
            this.AppCredentials = credentials;
            this.AccessToken = accessToken;

            this.BandInterceptorPipeline = new List<IBandInterceptor>();
            
            if (interceptor != null)
            {
                this.BandInterceptorPipeline.Add(interceptor);
            }

            ConfigureTokenManager(tokenManager);

            //Auto refresh should always be the last handle to be registered.
            ConfigureAutoRefresh(enableOAuth2TokenRefresh);

            CreateHttpClientForOAuth2();
        }

        private void ConfigureAutoRefresh(bool enableOAuth2TokenRefresh)
        {
            this.OAuth2TokenAutoRefresh = enableOAuth2TokenRefresh;
            if (OAuth2TokenAutoRefresh)
                this.BandInterceptorPipeline.Add(new OAuth2AutoRefreshInterceptor());
            return;
        }

        /// <summary>
        /// Simplest constructor for OAuth2- requires the minimum information required by FitBit.Net client to make succesful calls to Fitbit Api
        /// </summary>
        /// <param name="credentials">Obtain this information from your developer dashboard. App credentials are required to perform token refresh</param>
        /// <param name="accessToken">Authenticate with Fitbit API using OAuth2. Authenticator2 class is a helper for this process</param>
        /// <param name="interceptor">An interface that enables sniffing all outgoing and incoming http requests from FitbitClient</param>
        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, List<IBandInterceptor> interceptors, bool enableOAuth2TokenRefresh = true, ITokenManager tokenManager = null)
        {
            this.AppCredentials = credentials;
            this.AccessToken = accessToken;

            this.BandInterceptorPipeline = new List<IBandInterceptor>();

            if (interceptors != null && interceptors.Count > 0)
                this.BandInterceptorPipeline.AddRange(interceptors);

            ConfigureTokenManager(tokenManager);

            //Auto refresh should always be the last handle to be registered.
            ConfigureAutoRefresh(enableOAuth2TokenRefresh);
            CreateHttpClientForOAuth2();
        }

        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, bool enableOAuth2TokenRefresh) : this(credentials, accessToken, null, enableOAuth2TokenRefresh) {}

        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, List<IBandInterceptor> interceptors, bool enableOAuth2TokenRefresh) : this(credentials, accessToken, interceptors, enableOAuth2TokenRefresh, null) {}

        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, List<IBandInterceptor> interceptors, ITokenManager tokenManager) : this(credentials, accessToken, interceptors, true, tokenManager) {}

        public BandClient(BandAppCredentials credentials, OAuth2AccessToken accessToken, IBandInterceptor interceptor, ITokenManager tokenManager) : this(credentials, accessToken, interceptor, true, tokenManager) {}

        /// <summary>
        /// Advanced mode for library usage. Allows custom creation of HttpClient to account for future authentication methods
        /// </summary>
        /// <param name="customFactory">A function or lambda expression who is in charge of creating th HttpClient. It takes as an argument a HttpMessageHandler which does wiring for IFitbitInterceptor. To use IFitbitInterceptor you must pass this HttpMessageHandler as anargument to the constuctor of HttpClient</param>
        /// <param name="interceptor">An interface that enables sniffing all outgoing and incoming http requests from FitbitClient</param>
        public BandClient(Func<HttpMessageHandler, HttpClient> customFactory, IBandInterceptor interceptor = null, ITokenManager tokenManager = null)
        {
            this.OAuth2TokenAutoRefresh = false;

            ConfigureTokenManager(tokenManager);
            this.HttpClient = customFactory(new BandHttpMessageHandler(this, interceptor));
        }

        private void ConfigureTokenManager(ITokenManager tokenManager)
        {
            //TokenManager = tokenManager ?? new DefaultTokenManager();
        }

        private void CreateHttpClientForOAuth2()
        {
            var pipeline = this.CreatePipeline(BandInterceptorPipeline);
            if (pipeline != null)
                this.HttpClient = new HttpClient(pipeline);
            else
                this.HttpClient = new HttpClient();

            ConfigureAuthorizationHeader();
        }

        private void ConfigureAuthorizationHeader()
        {
            AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", AccessToken.Token);
            HttpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }

        public async Task<OAuth2AccessToken> RefreshOAuth2TokenAsync()
        {
            AccessToken = await TokenManager.RefreshTokenAsync(this);
            return AccessToken;
        }

        //private string FormatKey(APICollectionType apiCollectionType, string format)
        //{
        //    string strValue = apiCollectionType == APICollectionType.user ? string.Empty : apiCollectionType.ToString();
        //    return string.IsNullOrWhiteSpace(strValue) ? strValue : string.Format(format, strValue);
        //}

        /// <summary>
        /// Pass a freeform url. Good for debuging pursposes to get the pure json response back.
        /// </summary>
        /// <param name="apiPath">Fully qualified path including the Fitbit domain.</param>
        /// <returns></returns>
        public async Task<string> GetApiFreeResponseAsync(string apiPath)
        {
            string apiCall = apiPath;

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// General error checking of the response before specific processing is done.
        /// </summary>
        /// <param name="response"></param>
        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                // assumption is error response from fitbit in the 4xx range  
                var errors = new JsonDotNetSerializer().ParseErrors(await response.Content.ReadAsStringAsync());

                // rate limit hit
                //if (429 == (int)response.StatusCode)
                //{
                //    // not sure if we can use 'RetryConditionHeaderValue' directly as documentation is minimal for the header
                //    var retryAfterHeader = response.Headers.GetValues("Retry-After").FirstOrDefault();
                //    if (retryAfterHeader != null)
                //    {
                //        int retryAfter;
                //        if (int.TryParse(retryAfterHeader, out retryAfter))
                //        {
                //            throw new FitbitRateLimitException(retryAfter, errors);
                //        }
                //    }
                //}

                // request exception parsing
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                        throw new BandRequestException(response, errors);
                }

                // if we've got here then something unexpected has occured
                throw new BandException($"An error has occured. Please see error list for details - {response.StatusCode}", errors);
            }
        }

        /// <summary>
        /// This API lets you get all Bands that a user has joined.
        /// </summary>
        /// <returns>List of <see cref="Band"/></returns>
        public async Task<List<Band>> GetBands()
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/bands?access_token={_accessToken.Token}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var bands = serializer.Deserialize<BandsResult>(responseBody).ResultData.BandsList;
            return bands;
        }

        /// <summary>
        /// This API lets you get posts of a specific Band. The list is sorted by last modified date.
        /// </summary>
        /// <returns>List of <see cref="Post"/></returns>
        public async Task<Posts> GetPosts(string bandKey, string locale = "en_US")
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/band/posts?access_token={_accessToken.Token}&locale={locale}&band_key={bandKey}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var posts = serializer.Deserialize<PostsResult>(responseBody).ResultData;
            return posts;
        }

        /// <summary>
        /// This API lets you get a specific post.
        /// </summary>
        /// <returns><see cref="Post"/></returns>
        public async Task<Post> GetPost(string bandKey, string postKey)
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/band/post?access_token={_accessToken.Token}&band_key={bandKey}&post_key={postKey}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var post = serializer.Deserialize<PostResult>(responseBody).ResultData.Post;
            return post;
        }

        public async Task<List<Comment>> GetComments(string bandKey, string postKey, string sort = default(string))
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This API lets you get albums of a specific Band. The list is sorted by created date.
        /// </summary>
        /// <returns>List of <see cref="Album"/></returns>
        public async Task<Albums> GetAlbums(string bandKey)
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/band/albums?access_token={_accessToken.Token}&band_key={bandKey}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var albums = serializer.Deserialize<AlbumsResult>(responseBody).ResultData;
            return albums;
        }

        /// <summary>
        /// This API lets you get photos in a specific album.
        /// </summary>
        /// <returns>List of <see cref="Photo"/></returns>
        public async Task<Photos> GetPhotos(string bandKey, string photoAlbumKey)
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/band/album/photos?access_token={_accessToken.Token}&band_key={bandKey}&photo_album_key={photoAlbumKey}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var photos = serializer.Deserialize<PhotosResult>(responseBody).ResultData;
            return photos;
        }

        /// <summary>
        /// With this API, you can check if a user has write/delete permission on a specific Band.
        /// Depending on the Band settings, users may not have a permission to write a post or a comment even though they are members of the Band.
        /// Please call this API first to check user permission before trying to write/delete something.
        /// </summary>
        /// <returns>List of <see cref="string"/></returns>
        public async Task<List<string>> CheckPermissions(string bandKey, string permissions)
        {
            var apiCall = BandClientHelperExtensions.ToFullUrl($"/band/permissions?access_token={_accessToken.Token}&band_key={bandKey}&permissions={permissions}");

            HttpResponseMessage response = await HttpClient.GetAsync(apiCall);
            await HandleResponse(response);
            string responseBody = await response.Content.ReadAsStringAsync();
            var serializer = new JsonDotNetSerializer();
            // TODO do something to check BAND's result codes...
            var bandPermissions = serializer.Deserialize<PermissionsResult>(responseBody).ResultData.Permissions;
            return bandPermissions;
        }
    }
}
