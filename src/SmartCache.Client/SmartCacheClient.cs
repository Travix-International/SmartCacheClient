using JitterMagic;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SmartCache.Client.Http;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCache.Client
{
    /// <inheritdoc/>
    public class SmartCacheClient : ISmartCacheClient
    {
        private readonly IHttpClientBuilder httpClientBuilder;
        private readonly IHttpClient httpClient;
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public SmartCacheClient(IHttpClientBuilder httpClientBuilder)
        {
            this.httpClientBuilder = httpClientBuilder;
            httpClient = httpClientBuilder.Build();
        }

        public async Task<T> GetAsync<T>(Uri uri, CancellationTokenSource cancellationTokenSource = null)
        {
            return await GetFromCacheOrSource(uri, () => GetResponseItemAndCacheDuration<T>(uri, cancellationTokenSource));
        }
        
        private async Task<T> GetFromCacheOrSource<T>(Uri uri, Func<Task<(T, TimeSpan)>> functionIfCacheMiss)
        {
            string key = uri.AbsoluteUri;

            if (string.IsNullOrWhiteSpace(key))
            {
                var (value, _) = await functionIfCacheMiss.Invoke();

                return value;
            }

            var wrappedItem = MemoryCacheExtensions.Default.Get(key) as CacheItemWrapper<T>;

            if (wrappedItem == null)
            {
                var (value, cacheDuration) = await functionIfCacheMiss.Invoke();

                if (cacheDuration.TotalSeconds <= 0)
                {
                    // don't cache, so return the object immediately
                    return value;
                }

                wrappedItem = new CacheItemWrapper<T>(value, cacheDuration);

                var options = new MemoryCacheEntryOptions { AbsoluteExpiration = DateTime.UtcNow.Add(cacheDuration) };

                MemoryCacheExtensions.Default.Set(key, wrappedItem, options);
            }

            return wrappedItem.Value;
        }

        private async Task<(T, TimeSpan)> GetResponseItemAndCacheDuration<T>(Uri uri, CancellationTokenSource cancellationTokenSource)
        {
            HttpResponseMessage response = await GetResponseAsync(uri, cancellationTokenSource);

            T value = await GetItemFromResponse<T>(response);

            TimeSpan cacheDuration = GetCacheDurationFromResponse(response);
            return (value, cacheDuration);
        }

        private async Task<T> GetItemFromResponse<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var streamReader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
            else
            {
                // set item to null if 404/500 or content is null
                return default(T);
            }
        }

        private async Task<HttpResponseMessage> GetResponseAsync(Uri uri, CancellationTokenSource cancellationTokenSource)
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

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}