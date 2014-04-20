using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace AmyDaveWedding.Helpers
{
    static class CacheUtils
    {
        public static object GetHttpRuntimeCached(string name, Func<object> defaultFunc)
        {
            return GetCached(System.Web.HttpRuntime.Cache, name, defaultFunc);
        }

        public static object GetCached(Cache cache, string name, Func<object> defaultFunc)
        {
            object value = cache[name];
            if (value == null && defaultFunc != null)
            {
                value = defaultFunc();
                if (value != null)
                {
                    cache[name] = value;
                }
            }
            return value;
        }

        public static async Task<object> GetHttpRuntimeCachedAsync(string name, Func<Task<object>> defaultFunc)
        {
            return await GetCachedAsync(System.Web.HttpRuntime.Cache, name, defaultFunc);
        }

        public static async Task<object> GetCachedAsync(Cache cache, string name, Func<Task<object>> defaultFunc)
        {
            object value = cache[name];
            if (value == null && defaultFunc != null)
            {
                value = await defaultFunc();
                if (value != null)
                {
                    cache[name] = value;
                }
            }
            return value;
        }
    }
}