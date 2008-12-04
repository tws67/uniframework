using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Uniframework.Security;

namespace Uniframework.Client
{
    /// <summary>
    /// 客户端缓存管理器
    /// </summary>
    public class ClientCacheManager
    {
        private static Dictionary<String, ClientCacheDataReciver> cacheRecivers = new Dictionary<String, ClientCacheDataReciver>();

        /// <summary>
        /// Registers the cache.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="data">The data.</param>
        /// <param name="dataUpdateEvent">The data update event.</param>
        /// <param name="param">The param.</param>
        public static void RegisterCache(MethodInfo method, object data, string dataUpdateEvent, object param)
        {
            string key = GetKey(method, param);
            if(cacheRecivers.ContainsKey(key))
                throw new Exception("系统已经注册了该缓存");
            cacheRecivers.Add(key, new ClientCacheDataReciver(dataUpdateEvent, data, method, param));
        }

        /// <summary>
        /// Gets the cached data.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public static object GetCachedData(MethodInfo method, object param)
        {
            return cacheRecivers[GetKey(method, param)].CachedData;
        }

        /// <summary>
        /// Determines whether the specified method has cache.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="param">The param.</param>
        /// <returns>
        /// 	<c>true</c> if the specified method has cache; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasCache(MethodInfo method, object param)
        {
            return cacheRecivers.ContainsKey(GetKey(method, param));
        }

        #region Assistant functions

        private static string GetKey(MethodInfo method, object param)
        {
            return SecurityUtility.HashObject(method) + SecurityUtility.HashObject(param);
        }

        #endregion
    }
}
