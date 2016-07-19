using System.Net.Http;

namespace SmartCache.Client.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient Create();

        IHttpClient Create(HttpClientHandler handler);
    }
}