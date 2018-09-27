using BAND.Models;
using BAND.OAuth2;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BAND.ExampleApi.Controllers
{
    public class BandController : AsyncController
    {
        private const string BandApiBaseUrl = "https://openapi.band.us/v2";
        private readonly BandAppCredentials _appCredentials;
        private BandClient _bandClient;
        
        public BandController()
        {
            _appCredentials = new BandAppCredentials
            {
                ClientId = ConfigurationManager.AppSettings["BandClientId"],
                ClientSecret = ConfigurationManager.AppSettings["BandClientSecret"]
            };
        }

        // GET: Band
        public ActionResult Index()
        {
            ViewBag.Title = "Band Page";

            return View();
        }

        public ActionResult Login()
        {
            var authenticator = new OAuth2Helper(_appCredentials, ConfigurationManager.AppSettings["BaseUrl"] + "/band/callback");
            string[] scopes = new string[] { "profile" };

            string authUrl = authenticator.GenerateAuthUrl(scopes, null);

            return Redirect(authUrl);
        }

        public async Task<ActionResult> Callback(string code)
        {
            var authenticator = new OAuth2Helper(_appCredentials, ConfigurationManager.AppSettings["BaseUrl"] + "/band/callback");

            OAuth2AccessToken accessToken = await authenticator.ExchangeAuthCodeForAccessTokenAsync(code);

            if (!string.IsNullOrEmpty(accessToken.Token))
            {
                _bandClient = GetBandClient(accessToken);
            }

            return RedirectToAction("Profile");
        }

        public new async Task<ActionResult> Profile()
        {
            ViewBag.Title = "BAND Profile"; 
            
            var bandClient = (BandClient) Session["BandClient"];
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var url = $"{BandApiBaseUrl}/profile?access_token={bandClient.AccessToken.Token}";

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PostmanRuntime", "7.3.0"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsAsync<ProfileResult>().Result;
                    ViewBag.Profile = result.ResultData;
                }
            }

            return View("Profile");
        }

        public async Task<ActionResult> Bands()
        {
            ViewBag.Title = "BAND Bands";

            var bandClient = GetBandClient();
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var bands = await bandClient.GetBands();
            ViewBag.Bands = bands;
            
            return View("Bands");
        }

        public async Task<ActionResult> Posts(string id)
        {
            ViewBag.Title = "BAND Posts";

            var bandClient = GetBandClient();
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var posts = await bandClient.GetPosts(id);
            ViewBag.Posts = posts.Items;
            ViewBag.Paging = posts.Paging;
            
            return View("Posts");
        }

        public async Task<ActionResult> Post(string bandKey, string postKey)
        {
            ViewBag.Title = "BAND Post";

            var bandClient = GetBandClient();
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var post = await bandClient.GetPost(bandKey, postKey);
            ViewBag.Post = post;

            //var albums = await bandClient.GetAlbums(bandKey);
            //if (albums.Items != null && albums.Items.Any())
            //{
            //    var photos = await bandClient.GetPhotos(bandKey, albums.Items.First().PhotoAlbumKey);
            //}

            //var permissions = await bandClient.CheckPermissions(bandKey, "posting,commenting,contents_deletion");

            return View("Post");
        }

        public async Task<ActionResult> Albums(string bandKey)
        {
            ViewBag.Title = "BAND Albums";

            var bandClient = GetBandClient();
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var albums = await bandClient.GetAlbums(bandKey);
            if (albums.Items.Any())
            {
                foreach (var album in albums.Items)
                {
                    album.BandKey = bandKey;
                }
            }

            ViewBag.Albums = albums.Items;
            ViewBag.Paging = albums.Paging;

            return View("Albums");
        }

        public async Task<ActionResult> Photos(string bandKey, string photoAlbumKey)
        {
            ViewBag.Title = "BAND Album Photos";

            var bandClient = GetBandClient();
            if (bandClient == null)
            {
                return RedirectToAction("Index");
            }

            var photos = await bandClient.GetPhotos(bandKey, photoAlbumKey);
            ViewBag.Photos = photos.Items;
            ViewBag.Paging = photos.Paging;

            return View("Photos");
        }

        /// <summary>
        /// HttpClient and hence FitbitClient are designed to be long-lived for the duration of the session. This method ensures only one client is created for the duration of the session.
        /// More info at: http://stackoverflow.com/questions/22560971/what-is-the-overhead-of-creating-a-new-httpclient-per-call-in-a-webapi-client
        /// </summary>
        /// <returns></returns>
        private BandClient GetBandClient(OAuth2AccessToken accessToken = null)
        {
            if (Session["BandClient"] == null)
            {
                if (accessToken != null)
                {
                    BandClient client = new BandClient(_appCredentials, accessToken);
                    Session["BandClient"] = client;
                    return client;
                }
                //throw new Exception("First time requesting a BandClient from the session you must pass the AccessToken.");
                return null;
            }
            return (BandClient)Session["BandClient"];
        }
    }
}