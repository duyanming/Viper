using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Anno.Repository
{
    public static class MemoryCacheGlobalService
    {
        static MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>获取不到返回null</returns>
        public static object GetCacheValue(string key)
        {
            if (key != null && _cache.TryGetValue(key, out var val))
            {
                return val;
            }
            return null;
        }/// <summary>
         /// 获取缓存值
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="key"></param>
         /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (key != null && _cache.TryGetValue(key, out T val))
            {
                return val;
            }
            return default(T);
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (key != null)
            {
                _cache.Remove(key);
            }
        }
        /// <summary>
        /// 添加缓存内容 缓存默认 10分钟
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetChacheValue(string key, object value)
        {
            if (key != null)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }
        }
        /// <summary>
        /// 添加缓存内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire">有效时间</param>
        public static void SetChacheValue(string key, object value, TimeSpan expire)
        {
            if (key != null)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = expire
                });
            }
        }
    }
    public class MemoryCacheService
    {
        readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>获取不到返回null</returns>
        public object GetCacheValue(string key)
        {
            if (key != null && _cache.TryGetValue(key, out var val))
            {
                return val;
            }
            return null;
        }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (key != null && _cache.TryGetValue(key, out T val))
            {
                return val;
            }
            return default(T);
        }
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(long key)
        {
            return Get<T>(key.ToString());
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (key != null)
            {
                _cache.Remove(key);
            }
        }
        /// <summary>
        /// 添加缓存内容 缓存默认 10分钟
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetChacheValue(string key, object value)
        {
            if (key != null)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                });
            }
        }
        /// <summary>
        /// 添加缓存内容 缓存默认 10分钟
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetChacheValue(long key, object value)
        {
            SetChacheValue(key.ToString(), value);
        }
        /// <summary>
        /// 添加缓存内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire">有效时间</param>
        public void SetChacheValue(string key, object value, TimeSpan expire)
        {
            if (key != null)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = expire
                });
            }
        }
    }
}
