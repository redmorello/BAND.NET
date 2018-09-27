using System.Threading.Tasks;

namespace BAND.OAuth2
{
    public interface ITokenManager
    {
        Task<OAuth2AccessToken> RefreshTokenAsync(BandClient client);
    }
}
