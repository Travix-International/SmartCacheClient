using System.Net.Http;

namespace SmartCache.Client.Http
{
    public class SmartCacheHttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Create(HttpMessageHandler handler)
        {
            return new SmartCacheHttpClient(handler);
        }
    }
}