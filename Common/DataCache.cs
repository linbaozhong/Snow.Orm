using System;
using System.Web;
using System.Web.Caching;

namespace Snow
{
  public class DataCache
  {

    public static object GetCache(string CacheKey)
    {
      return HttpRuntime.Cache[CacheKey];
    }

    public static void SetCache(string CacheKey, object objObject)
    {
      HttpRuntime.Cache.Insert(CacheKey, objObject);
    }

    public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
    {
      HttpRuntime.Cache.Insert(CacheKey, objObject, (CacheDependency) null, absoluteExpiration, slidingExpiration);
    }

    public static void DeleteCache(string CacheKey)
    {
      HttpRuntime.Cache.Remove(CacheKey);
    }
  }
}
