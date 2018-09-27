using BAND.OAuth2;
using System.Threading.Tasks;

namespace BAND
{
    public interface ITokenManager
    {
        Task<OAuth2AccessToken> RefreshTokenAsync(BandClient client);
    }
}
