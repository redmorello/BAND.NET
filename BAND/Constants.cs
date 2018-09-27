namespace BAND
{
    internal class Constants
    {
        public const string BaseApiUrl = "https://openapi.band.us/v2/";
        public const string TemporaryCredentialsRequestTokenUri = "oauth/request_token";
        public const string TemporaryCredentialsAccessTokenUri = "oauth/access_token";
        public const string AuthorizeUri = "oauth/authorize";
        public const string LogoutAndAuthorizeUri = "oauth/logout_and_authorize";

        public class Headers
        {
        }

        public class Formatting
        {
            public const string TrailingSlash = "{0}/";
            public const string LeadingDash = "-{0}";
        }
    }
}
