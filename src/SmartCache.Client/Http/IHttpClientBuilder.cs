namespace SmartCache.Client.Http
{
    public interface IHttpClientBuilder
    {
        IHttpClient HttpClient { get; }
    }
}