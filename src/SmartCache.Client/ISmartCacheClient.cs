using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCache.Client
{
    /// <summary>
    /// Http client that caches deserialized responses per full url and uses the max-age response header as cache duration
    /// </summary>
    public interface ISmartCacheClient
    {
        /// <summary>
        /// Retrieve an item of type T from cache or uri
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(Uri uri, CancellationTokenSource cancellationTokenSource = null) where T : class;
    }
}