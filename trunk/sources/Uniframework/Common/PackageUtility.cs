using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Uniframework.Services;

namespace Uniframework
{
    /// <summary>
    /// 远程调用包工具类
    /// </summary>
    public static class PackageUtility
    {
        private static Serializer serializer = new Serializer();

        /// <summary>
        /// 远程方法调用封装常量
        /// </summary>
        public static readonly string METHOD                  = "context_uf_method"; // 方法体
        public static readonly string METHOD_NAME             = "context_uf_method_name"; // 方法名
        public static readonly string METHOD_PARAMETERS       = "context_uf_method_parameters"; // 方法参数
        public static readonly string METHOD_ISGENERIC        = "context_uf_method_isgeneric"; // 是否是泛型方法
        public static readonly string METHOD_INVOKEINFO       = "context_uf_invokeinfo"; // 方法调用体信息用于在服务器端生成泛型方法体

        public static readonly string SESSION_ID              = "context_uf_session_id"; // 会话标识
        public static readonly string SESSION_USERNAME        = "context_uf_session_username"; // 用户标识
        public static readonly string SESSION_PASSWORD        = "context_uf_session_password"; // 用户密码
        public static readonly string SESSION_IPADDRESS       = "context_uf_session_ipaddress"; // 客户端IP地址
        public static readonly string SESSION_ENCTRYPTKEY     = "context_uf_session_encryptkey"; // 密钥

        /// <summary>
        /// 
        /// </summary>
        public static readonly string PARAMETERS_NAME = "contex_uf_parameters";


        ///// <summary>
        ///// 编码远程过程调用包
        ///// </summary>
        ///// <param name="method">The method.</param>
        ///// <param name="parameters">The parameters.</param>
        ///// <param name="package">The package.</param>
        //public static void EncodeInvoke(MethodInfo method, object parameters, string encryptKey, ref NetworkInvokePackage package)
        //{
        //    package.Context[METHOD] = serializer.Serialize<MethodInfo>(method);
        //    package.Context[METHOD_NAME] = serializer.Serialize<string>(method.Name);
        //    package.Context[METHOD_PARAMETERS] = SecurityUtility.DESEncrypt(serializer.Serialize<object>(parameters), encryptKey);
        //}

        /// <summary>
        /// 解码远程过程调用包
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        public static void DecodeInvoke(NetworkInvokePackage package, string encryptKey, out MethodInfo method, out object[] parameters)
        {
            method = serializer.Deserialize<MethodInfo>((byte[])package.Context[METHOD]);
            parameters = serializer.Deserialize<object[]>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_PARAMETERS], encryptKey));
        }

        /// <summary>
        /// 解码远程过程调用包
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="invokeInfo">The invoke info.</param>
        public static void DecodeInvoke(NetworkInvokePackage package, string encryptKey, out MethodInvokeInfo invokeInfo, out object[] parameters)
        {
            invokeInfo = serializer.Deserialize<MethodInvokeInfo>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_INVOKEINFO], encryptKey));
            parameters = serializer.Deserialize<object[]>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_PARAMETERS], encryptKey)); 
        }

        /// <summary>
        /// Encodes the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="package">The package.</param>
        public static void EncodeInvoke(MethodInfo method, object parameters, string encryptKey, ref NetworkInvokePackage package)
        {
            package.Context[METHOD]            = serializer.Serialize<MethodInfo>(method);
            package.Context[METHOD_NAME]       = serializer.Serialize<string>(method.Name);
            package.Context[METHOD_ISGENERIC]  = method.IsGenericMethod;
            package.Context[METHOD_PARAMETERS] = SecurityUtility.DESEncrypt(serializer.Serialize<object>(parameters), encryptKey);
            package.Context[METHOD_INVOKEINFO] = SecurityUtility.DESEncrypt(serializer.Serialize<MethodInvokeInfo>(new MethodInvokeInfo(method)), encryptKey);
        }
    }
}
