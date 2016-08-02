using System;
using System.Runtime.Caching;

namespace Snow
{
    public static class DataCache
    {
        private static ObjectCache Cache = MemoryCache.Default;

        public static object Get(string CacheKey)
        {
            return Cache.Get(CacheKey);
        }

        public static void Set(string CacheKey, object obj)
        {
            Cache.Set(CacheKey, obj, DateTime.Now.AddMinutes(30));
        }

        public static void Set(string CacheKey, object obj, DateTime absoluteExpiration)
        {
            Cache.Set(CacheKey, obj, new DateTimeOffset(absoluteExpiration));
        }

        public static void Remove(string CacheKey)
        {
            Cache.Remove(CacheKey);
        }
    }
}
