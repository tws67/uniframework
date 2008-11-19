using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;

using Uniframework.Client.OfflineProxy;
using Uniframework.Services;

namespace Uniframework.Client
{
    public class ServiceProxyInterceptor : IInterceptor
    {
        private Serializer serializer;
        private DefaultServiceAgent serviceAgent;

        public ServiceProxyInterceptor()
        {
            serializer = new Serializer();
        }

        public DefaultServiceAgent ServiceAgent
        {
            get { return serviceAgent; }
            set { serviceAgent = value; }
        }

        public object InvokeCommand(MethodInfo method, object parameters)
        {
            return CommunicateProxy.InvokeCommand(method, parameters);
        }

        public object JoinParameterType(object parameters, object types)
        {
            object[] pat = new object[2];
            pat[0] = parameters;
            pat[1] = types;
            return pat;
        }

        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method = invocation.Method;
            RemoteMethodInfo remoteMethod = null;
            if (method.IsGenericMethod) {
                method = invocation.GetConcreteMethodInvocationTarget();

                if (method.DeclaringType == typeof(IRemoteCaller)) {
                    invocation.ReturnValue = method.Invoke(invocation.InvocationTarget, invocation.GenericArguments);
                    return;
                }
            }
            else {
                if (method.DeclaringType == typeof(IRemoteCaller)) {
                    invocation.ReturnValue = method.Invoke(invocation.InvocationTarget, invocation.Arguments);
                    return;
                }
            }

            remoteMethod = InterfaceConfigLoader.GetServiceInfo(method);
            if (!remoteMethod.Offline)
            {
                // Modified : 修复系统对缓存方法的调用
                // Jacky 2007-05-22 11:28
                // begin modified
                if (invocation.Method.IsDefined(typeof(ClientCacheAttribute), true))
                {
                    if (ClientCacheManager.HasCache(invocation.Method, GetArgs(invocation)))
                        invocation.ReturnValue = ClientCacheManager.GetCachedData(invocation.Method, GetArgs(invocation));
                    else
                    {
                        object result = InvokeCommand(invocation.Method, GetArgs(invocation));
                        ClientCacheManager.RegisterCache(invocation.Method, result, remoteMethod.DataUpdateEvent, GetArgs(invocation));
                        invocation.ReturnValue = result;
                    }
                }
                else // end modified
                    invocation.ReturnValue = InvokeCommand(invocation.Method, GetArgs(invocation));
            }
            else
            {
                if (invocation.Method.IsDefined(typeof(ClientCacheAttribute), true))
                {
                    if (ClientCacheManager.HasCache(invocation.Method, GetArgs(invocation)))
                        invocation.ReturnValue = ClientCacheManager.GetCachedData(invocation.Method, GetArgs(invocation));
                    else
                    {
                        object result = InvokeCommand(invocation.Method, GetArgs(invocation));
                        ClientCacheManager.RegisterCache(invocation.Method, result, remoteMethod.DataUpdateEvent, GetArgs(invocation));
                        invocation.ReturnValue = result;
                    }
                }
                else
                    OfflineProcess(invocation.Method, GetArgs(invocation)); // 调用离线处理方法
            }
        }

        //public object Intercept(IInvocation invocation, params object[] args)
        //{
        //    foreach (object obj in args)
        //    {
        //        SetDataSetSerializationFormat(obj);
        //    }

        //    if (invocation.Method.DeclaringType == typeof(IRemoteCaller))
        //        return invocation.Proceed(args);

        //    RemoteMethodInfo remoteMethodInfo = InterfaceConfigLoader.GetServiceInfo(invocation.Method);
        //    if (!remoteMethodInfo.Offline)
        //    {
        //        // Modified : 修复系统对缓存方法的调用
        //        // Jacky 2007-05-22 11:28
        //        // begin modified
        //        if (invocation.Method.IsDefined(typeof(ClientCacheAttribute), true))
        //        {
        //            if (ClientCacheManager.HasCache(invocation.Method, args))
        //                return ClientCacheManager.GetCachedData(invocation.Method, args);
        //            else
        //            {
        //                object result = InvokeCommand(invocation.Method, args);
        //                ClientCacheManager.RegisterCache(invocation.Method, result, remoteMethodInfo.DataUpdateEvent, args);
        //                return result;
        //            }
        //        }
        //        else // end modified
        //            return InvokeCommand(invocation.Method, args);
        //    }
        //    else
        //    {
        //        if (invocation.Method.IsDefined(typeof(ClientCacheAttribute), true))
        //        {
        //            if (ClientCacheManager.HasCache(invocation.Method, args))
        //                return ClientCacheManager.GetCachedData(invocation.Method, args);
        //            else
        //            {
        //                object result = InvokeCommand(invocation.Method, args);
        //                ClientCacheManager.RegisterCache(invocation.Method, result, remoteMethodInfo.DataUpdateEvent, args);
        //                return result;
        //            }
        //        }
        //        else
        //            OfflineProcess(invocation.Method, args); // 调用离线处理方法
        //        return null;
        //    }
        //}

        #endregion

        #region Assistant function

        private object[] GetArgs(IInvocation invocation)
        {
            return invocation.Method.IsGenericMethod ? invocation.GenericArguments : invocation.Arguments;
        }

        private void ThrowException(string message, IInvocation invocation, string[] parameterTypes)
        {
            string errorMessage = message + "\nServiceType:" + invocation.Method.ReflectedType.ToString() + "\nMethod:" + invocation.Method.Name + "\nParameter:";
            foreach (string parameterType in parameterTypes)
            {
                errorMessage += parameterType + "\n";
            }
            throw new Exception(errorMessage);
        }

        private void OfflineProcess(MethodInfo method, params object[] parameters)
        {
            Request request = new Request(method, serviceAgent, parameters);
            RequestManager.Instance.RequestQueue.Enqueue(request);
        }

        private void SetDataSetSerializationFormat(object obj)
        {
            if (obj is DataTable)
            {
                DataTable dt = (DataTable)obj;
                dt.RemotingFormat = SerializationFormat.Binary;
            }
            else if (obj is DataSet)
            {
                DataSet ds = (DataSet)obj;
                ds.RemotingFormat = SerializationFormat.Binary;
            }
        }

        private string GetPathAviliableKey(object obj)
        {
            return SecurityUtility.HashObject(obj).Replace("/", "!");
        }

        #endregion
    }
}
