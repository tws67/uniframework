using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Client
{
    //public class ClientCacheManager
    //{
    //    private static Dictionary<string, ClientCacheDataReciver> cacheRecivers = new Dictionary<string, ClientCacheDataReciver>();

    //    public static void RegisterCache(MethodInfo method, object data, string dataUpdateEvent, object param)
    //    {
    //        string key = GetKey(method, param);
    //        if (cacheRecivers.ContainsKey(key)) throw new Exception("ϵͳ�Ѿ�ע���˸û���");
    //        cacheRecivers.Add(key, new ClientCacheDataReciver(dataUpdateEvent, data, method, param));
    //    }

    //    private static string GetKey(MethodInfo method, object param)
    //    {
    //        return SecurityHelper.HashObject(method) + SecurityHelper.HashObject(param);
    //    }

    //    public static object GetCachedData(MethodInfo method, object param)
    //    {
    //        return cacheRecivers[GetKey(method, param)].CachedData;
    //    }

    //    public static bool HasCache(MethodInfo method, object param)
    //    {
    //        return cacheRecivers.ContainsKey(GetKey(method, param));
    //    }
    //}

    /// <summary>
    /// �ͻ��˻��������
    /// </summary>
    public class ClientCacheManager
    {
        private static Dictionary<String, ClientCacheDataReciver> cacheRecivers = new Dictionary<String, ClientCacheDataReciver>();

        public static void RegisterCache(MethodInfo method, object data, string dataUpdateEvent, object param)
        {
            string key = GetKey(method, param);
            if(cacheRecivers.ContainsKey(key))
                throw new Exception("ϵͳ�Ѿ�ע���˸û���");
            cacheRecivers.Add(key, new ClientCacheDataReciver(dataUpdateEvent, data, method, param));
        }

        public static object GetCachedData(MethodInfo method, object param)
        {
            return cacheRecivers[GetKey(method, param)].CachedData;
        }

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
