using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 方法调用信息，封装客户端调用远程方法时传给服务器的信息
    /// </summary>
    [Serializable]
    public class MethodInvokeInfo
    {
        private string name;
        private Type declaringType;
        private Type[] genericArguments;
        private ParameterInfo[] parameters;
        private Type returnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvokeInfo"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public MethodInvokeInfo(MethodInfo method)
        {
            name = method.Name;
            declaringType = method.DeclaringType;
            genericArguments = method.GetGenericArguments();
            parameters = method.GetParameters();
            returnType = method.ReturnType;
        }

        /// <summary>
        /// Gets the generic method
        /// </summary>
        /// <returns></returns>
        public MethodInfo GetGenericMethod()
        {
            MethodInfo genericMethod = null;
            if (parameters == null) {
                MethodInfo method = declaringType.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
                genericMethod = method.MakeGenericMethod(genericArguments);
            }
            else {
                foreach (MethodInfo method in declaringType.GetMethods(BindingFlags.Instance | BindingFlags.Public)) {
                    if (method.Name == name) {
                        genericMethod = method.MakeGenericMethod(genericArguments); // 生成泛型方法定义

                        // 比较方法参数
                        ParameterInfo[] parameterInfos = genericMethod.GetParameters();
                        if (parameterInfos.Length == parameters.Length) {
                            for (int i = 0; i < parameters.Length; i++) {
                                if (parameterInfos[i].ParameterType != parameters[i].ParameterType)
                                    continue;
                            }
                        }

                        break; // 找到需要的泛型方法定义
                    }
                }
            }
            return genericMethod;
        }

        #region Members

        public string Name
        {
            get { return name; }
        }

        public Type DeclaringType
        {
            get { return declaringType; }
        }

        public Type[] GenericArguments
        {
            get { return genericArguments; }
        }

        public ParameterInfo[] Parameters
        {
            get { return parameters; }
        }

        public Type ReturnType
        {
            get { return returnType; }
        }

        #endregion
    }
}
