namespace SmartCache.Client.Http
{
    public interface IHttpClientFactory
    {
        IHttpClient Create();
    }
}