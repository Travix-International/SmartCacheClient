using System.Net.Http;

namespace SmartCache.Client.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient Create(HttpMessageHandler handler);
    }
}