﻿using System;
using Microsoft.Extensions.Caching.Memory;

namespace SmartCacheClient.Dotnet
{
    public static class MemoryCacheExtensions
    {
        private static readonly Lazy<MemoryCache> lazy = new Lazy<MemoryCache>(
            () => new MemoryCache(new MemoryCacheOptions()));

        public static MemoryCache Default
        {
            get
            {
                return lazy.Value;
            }
        }
    }
}