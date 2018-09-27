using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BAND
{
    public interface IBandInterceptor
    {
        Task<HttpResponseMessage> InterceptRequest(HttpRequestMessage request, CancellationToken cancellationToken, BandClient invokingClient);
        Task<HttpResponseMessage> InterceptResponse(Task<HttpResponseMessage> response, CancellationToken cancellationToken, BandClient invokingClient);
    }
}
