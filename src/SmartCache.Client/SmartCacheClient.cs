using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JitterMagic;
using Microsoft.Extensions.Caching.Memory;
using SmartCache.Client.Certificates;
using SmartCache.Client.Http;

namespace SmartCache.Client
{
    /// <inheritdoc/>
    public class SmartCacheClient : ISmartCacheClient
    {
        private readonly IClientCertificateProvider clientCertificateProvider;

        private readonly IHttpClientFactory httpClientFactory;

        public SmartCacheClient(IHttpClientFactory httpClientFactory, IClientCertificateProvider clientCertificateProvider)
        {
            this.httpClientFactory = httpClientFactory;
            this.clientCertificateProvider = clientCertificateProvider;
        }

        public async Task<T> GetAsync<T>(Uri uri, CancellationTokenSource cancellationTokenSource = null) where T : class
        {
            return await GetFromCacheOrSource(uri, () => GetResponseItemAndCacheDuration<T>(uri, cancellationTokenSource));
        }
        
        private async Task<T> GetFromCacheOrSource<T>(Uri uri, Func<Task<Tuple<T, TimeSpan>>> functionIfCacheMiss) where T : class
        {
            string key = uri.AbsoluteUri;

            if (string.IsNullOrWhiteSpace(key))
            {
                Tuple<T, TimeSpan> tuple = await functionIfCacheMiss.Invoke();

                return tuple.Item1;
            }

            var wrappedItem = MemoryCacheExtensions.Default.Get(key) as CacheItemWrapper<T>;

            if (wrappedItem == null)
            {
                Tuple<T, TimeSpan> tuple = await functionIfCacheMiss.Invoke();

                if (tuple.Item2.TotalSeconds <= 0)
                {
                    // don't cache, so return the object immediately
                    return tuple.Item1;
                }

                wrappedItem = new CacheItemWrapper<T>(tuple.Item1, tuple.Item2);

                var options = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.Add(tuple.Item2) };

                MemoryCacheExtensions.Default.Set(key, wrappedItem, options);
            }

            return wrappedItem.Value;
        }

        private async Task<Tuple<T, TimeSpan>> GetResponseItemAndCacheDuration<T>(Uri uri, CancellationTokenSource cancellationTokenSource) where T : class
        {
            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage response = await GetResponseAsync(httpClient, uri, cancellationTokenSource);

                T value = await GetItemFromResponse<T>(response);

                TimeSpan cacheDuration = GetCacheDurationFromResponse(response);
                return new Tuple<T, TimeSpan>(value, cacheDuration);
            }
        }

        private async Task<T> GetItemFromResponse<T>(HttpResponseMessage response) where T : class
        {
            // set item to null if 404/500 or content is null
            T item = response.StatusCode == HttpStatusCode.OK && response.Content != null
                ? await response.Content.ReadAsAsync<T>()
                : null;

            return item;
        }

        private async Task<HttpResponseMessage> GetResponseAsync(IHttpClient httpClient, Uri uri, CancellationTokenSource cancellationTokenSource)
        {
            return cancellationTokenSource != null
                ? await httpClient.GetAsync(uri, cancellationTokenSource.Token)
                : await httpClient.GetAsync(uri);
        }

        private TimeSpan GetCacheDurationFromResponse(HttpResponseMessage response)
        {
            var maxAge = response.Headers.CacheControl?.MaxAge;

            return maxAge ?? TimeSpan.FromSeconds(Jitter.Apply(60));
        }

        private IHttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();

            handler.ClientCertificates.AddRange(clientCertificateProvider.GetCertificates().ToArray());

            return httpClientFactory.Create(handler);
        }
    }
}