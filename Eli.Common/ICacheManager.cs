using System;

namespace Eli.Common
{
    public interface ICacheManager
    {
        void Add(object cacheObject, string key);
        void Remove(string key);
        bool Exist(string key);
        T Get<T>(string key) where T : class;
        T Get<T>(string key, Func<T> fn) where T : class;
        T Get<T>(string key, double timeoutminutes, Func<T> fn) where T : class;
    }
}
