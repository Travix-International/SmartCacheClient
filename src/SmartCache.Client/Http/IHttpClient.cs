using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCache.Client.Http
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(Uri requestUri);

        Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken);
    }
}