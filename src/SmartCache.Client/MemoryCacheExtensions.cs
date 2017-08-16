using System;
using Microsoft.Extensions.Caching.Memory;

namespace SmartCache.Client
{
    internal static class MemoryCacheExtensions
    {
        private static readonly Lazy<MemoryCache> lazy = new Lazy<MemoryCache>(
            () => new MemoryCache(new MemoryCacheOptions()));

        public static MemoryCache Default => lazy.Value;
    }
}