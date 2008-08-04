using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 远程调用包工具类
    /// </summary>
    public static class PackageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string METHOD_NAME = "contex_method";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string PARAMETERS_NAME = "contex_parameters";

        /// <summary>
        /// 封装远程过程调用包
        /// </summary>
        /// <param name="package">包实例</param>
        /// <param name="method">方法信息</param>
        /// <param name="parameters">参数信息</param>
        public static void EncodeInvoke(NetworkInvokePackage package, byte[] method, byte[] parameters)
        {
            package.Context[METHOD_NAME] = method;
            package.Context[PARAMETERS_NAME] = parameters;
        }
    }
}
