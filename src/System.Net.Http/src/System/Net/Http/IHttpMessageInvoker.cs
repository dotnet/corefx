using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public interface IHttpMessageInvoker
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
