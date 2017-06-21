using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JitterMagic;
using Microsoft.Extensions.Caching.Memory;
using SmartCache.Client.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace SmartCache.Client
{
    /// <inheritdoc/>
    public class SmartCacheClient : ISmartCacheClient
    {
        private readonly IHttpClientBuilder httpClientBuilder;
        private readonly MediaTypeFormatter halPlusJsonFormatter;

        public SmartCacheClient(IHttpClientBuilder httpClientBuilder)
        {
            this.httpClientBuilder = httpClientBuilder;

            halPlusJsonFormatter = new JsonMediaTypeFormatter();
            halPlusJsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
        }

        public async Task<T> GetAsync<T>(Uri uri, CancellationTokenSource cancellationTokenSource = null)
        {
            return await GetFromCacheOrSource(uri, () => GetResponseItemAndCacheDuration<T>(uri, cancellationTokenSource));
        }
        
        private async Task<T> GetFromCacheOrSource<T>(Uri uri, Func<Task<Tuple<T, TimeSpan>>> functionIfCacheMiss)
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

        private async Task<Tuple<T, TimeSpan>> GetResponseItemAndCacheDuration<T>(Uri uri, CancellationTokenSource cancellationTokenSource)
        {
            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage response = await GetResponseAsync(httpClient, uri, cancellationTokenSource);

                T value = await GetItemFromResponse<T>(response);

                TimeSpan cacheDuration = GetCacheDurationFromResponse(response);
                return new Tuple<T, TimeSpan>(value, cacheDuration);
            }
        }

        private async Task<T> GetItemFromResponse<T>(HttpResponseMessage response)
        {
            // set item to null if 404/500 or content is null
            T item = response.StatusCode == HttpStatusCode.OK && response.Content != null
                ? await response.Content.ReadAsAsync<T>(new List<MediaTypeFormatter> { halPlusJsonFormatter })
                : default(T);

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
            return httpClientBuilder.Build();
        }
    }
}