using System;
using System.Web;
using System.Web.Caching;

namespace Eli.Common
{
    public sealed class CacheManager : ICacheManager
    {
/*
        private static object _cacheLock = new object();
*/

        public CacheManager()
        {
            TimeoutMinutes = 60;
            AbsoluteExpiration = Cache.NoAbsoluteExpiration;
            Dependency = null;
            ItemPriority = CacheItemPriority.Default;
            ItemRemovedCallback = null;
        }

        private double TimeoutMinutes { get; set; }
        private DateTime AbsoluteExpiration { get; set; }
        private CacheDependency Dependency { get; set; }
        private CacheItemPriority ItemPriority { get; set; }
        private CacheItemRemovedCallback ItemRemovedCallback { get; set; }

        public void Add(object cacheObject, string key)
        {
            Add(cacheObject, key, TimeoutMinutes);
        }

        private void Add(object cacheObject, string key, double minutes)
        {
            HttpContext.Current.Cache.Insert(key, cacheObject,
                                    Dependency,
                                    AbsoluteExpiration,
                                    TimeSpan.FromMinutes(minutes),
                                    ItemPriority,
                                    ItemRemovedCallback);
        }

        public void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public bool Exist(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }

        public T Get<T>(string key) where T : class
        {
            return HttpContext.Current.Cache[key] as T;
        }

        public T Get<T>(string key, Func<T> fn) where T : class
        {
            return Get(key, TimeoutMinutes, fn);
        }

        public T Get<T>(string key, double timeoutminutes, Func<T> fn) where T : class
        {
            var obj = Get<T>(key);
            if (obj == default(T))
            {
                // PRM - Disabled locking for now until we find a better locking strategy.. 
                //lock (_CacheLock)
                //{
                //    // Re-check for object since we might have been blocked by the lock
                //    obj = this.Get<T>(key);
                //    if (obj == default(T))
                //    {
                obj = fn();
                Add(obj, key, timeoutminutes);
                //    }
                //}
            }

            return obj;
        }
    }
}
