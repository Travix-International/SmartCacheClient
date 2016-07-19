using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCache.Client.Http
{
    public class SmartCacheHttpClient : IHttpClient
    {
        private readonly HttpClient client;

        public SmartCacheHttpClient()
        {
            client = new HttpClient();
        }

        public SmartCacheHttpClient(HttpClientHandler handler)
        {
            client = new HttpClient(handler);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return client.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            return client.GetAsync(requestUri, cancellationToken);
        }
    }
}