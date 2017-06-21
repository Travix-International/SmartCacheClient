using System;

namespace SmartCache.Client
{
    /// <summary>
    /// Internal wrapper in order to allow to store null values in cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CacheItemWrapper<T>
    {
        public T Value { get; private set; }

        public DateTime Inserted { get; private set; }

        public TimeSpan CacheDuration { get; private set; }

        public CacheItemWrapper(T value, TimeSpan cacheDuration)
        {
            Value = value;
            Inserted = DateTime.UtcNow;
            CacheDuration = cacheDuration;
        }
    }
}