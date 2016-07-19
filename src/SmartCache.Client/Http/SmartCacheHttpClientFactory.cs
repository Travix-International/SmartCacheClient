using System.Net.Http;

namespace SmartCache.Client.Http
{
    public class SmartCacheHttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Create(HttpClientHandler handler)
        {
            return new SmartCacheHttpClient(handler);
        }
    }
}