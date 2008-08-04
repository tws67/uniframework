using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 服务调用接口
    /// </summary>
    public interface IServiceCaller
    {
        /// <summary>
        /// 通过反射调用指定方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="parameters">方法参数</param>
        /// <returns></returns>
        object Invoke(MethodInfo method, object[] parameters);
    }
}
