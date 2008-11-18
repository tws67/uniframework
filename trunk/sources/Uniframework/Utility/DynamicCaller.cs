using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Uniframework
{
    public delegate object DynamicInvoker(object target, object[] paramters);

    /// <summary>
    /// 快速反射调用工具类
    /// </summary>
    public static class DynamicCaller
    {
        /// <summary>
        /// 获取方法定义的快速调用器
        /// </summary>
        /// <param name="methodInfo">方法信息</param>
        /// <returns>快速调用委托<see cref="FastInvokeHandler"/></returns>
        public static DynamicInvoker GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), 
                new Type[] { typeof(object), typeof(object[]) }, 
                methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();

            // 获取方法参数信息
            ParameterInfo[] ps = methodInfo.GetParameters();
            if (!methodInfo.IsGenericMethod)
            {
                Type[] paramTypes = new Type[ps.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (ps[i].ParameterType.IsByRef)
                        paramTypes[i] = ps[i].ParameterType.GetElementType();
                    else
                        paramTypes[i] = ps[i].ParameterType;
                }
                LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

                // 为每个参数生成本地变量
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    locals[i] = il.DeclareLocal(paramTypes[i], true);
                }
                // 将方法类型信息转存到本地变量中
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    EmitCastToReference(il, paramTypes[i]);
                    il.Emit(OpCodes.Stloc, locals[i]);
                }

                // 对象入栈
                if (!methodInfo.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }

                // 将方法参数的本地副本入栈
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (ps[i].ParameterType.IsByRef)
                        il.Emit(OpCodes.Ldloca_S, locals[i]);
                    else
                        il.Emit(OpCodes.Ldloc, locals[i]);
                }

                if (methodInfo.IsStatic)
                    il.EmitCall(OpCodes.Call, methodInfo, null);
                else
                    il.EmitCall(OpCodes.Callvirt, methodInfo, null);

                if (methodInfo.ReturnType == typeof(void))
                    il.Emit(OpCodes.Ldnull);
                else
                    EmitBoxIfNeeded(il, methodInfo.ReturnType);

                // 更新引用类参数以保证其可以正确返回
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    if (ps[i].ParameterType.IsByRef)
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        EmitFastInt(il, i);
                        il.Emit(OpCodes.Ldloc, locals[i]);
                        if (locals[i].LocalType.IsValueType)
                            il.Emit(OpCodes.Box, locals[i].LocalType);
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                }

                // 返回
                il.Emit(OpCodes.Ret);
            }
            else {
                // define local vars
                il.DeclareLocal(typeof(object));

                // load first argument, the instace where the method is to be invoked
                il.Emit(OpCodes.Ldarg_0);

                // cast to the correct type
                il.Emit(OpCodes.Castclass, methodInfo.DeclaringType);

                for (int i = 0; i < ps.Length; i++) {
                    // load paramters they are passed as an object array
                    il.Emit(OpCodes.Ldarg_1);

                    // load array element
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelem_Ref);

                    // cast or unbox parameter as needed
                    Type parameterType = ps[i].ParameterType;
                    if (parameterType.IsClass) {
                        il.Emit(OpCodes.Castclass, parameterType);
                    }
                    else {
                        il.Emit(OpCodes.Unbox_Any, parameterType);
                    }
                }

                // call method
                il.EmitCall(OpCodes.Call, methodInfo, null);

                // handle method return if needed
                if (methodInfo.ReturnType == typeof(void)) {
                    // return null
                    il.Emit(OpCodes.Ldnull);
                }
                else {
                    // box value if needed
                    if (methodInfo.ReturnType.IsValueType) {
                        il.Emit(OpCodes.Box, methodInfo.ReturnType);
                    }
                }

                // store to the local var
                il.Emit(OpCodes.Stloc_0);

                // load local and return
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
            }
            DynamicInvoker invoker = (DynamicInvoker)dynamicMethod.CreateDelegate(typeof(DynamicInvoker));
            return invoker;
        }

        #region Generate generic method

        public static DynamicInvoker MethodGenerator(MethodInfo method)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object),
                new Type[] { typeof(object), typeof(object[]) },
                method.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();

            Type[] types = GetParameterTypes(method.GetParameters());
            Type[] gtypes;
            MethodInfo callmethod;
            il.DeclareLocal(typeof(MethodBase));
            il.DeclareLocal(typeof(object[]));
            bool isvoid = method.ReturnType.ToString() == "System.Void";
            if (!isvoid) // 定义返回值变量
            {
                il.DeclareLocal(GetType(method.ReturnType));
            }
            il.DeclareLocal(typeof(object[]));
            il.DeclareLocal(typeof(Type[]));
            il.Emit(OpCodes.Nop);
            callmethod = typeof(MethodBase).GetMethod("GetCurrentMethod", new Type[0]);
            il.Emit(OpCodes.Call, callmethod);
            il.Emit(OpCodes.Stloc_0); // 把调用GetCurrentMethod的返回值封送到局部变量0里

            #region AddGenericTypes
            if (method.IsGenericMethod) // 顶他个肺是泛型方法
            {
                gtypes = method.GetGenericArguments();  // 得到泛型参类型
                il.Emit(OpCodes.Ldc_I4, gtypes.Length); // 加载数组长度
                il.Emit(OpCodes.Newarr, typeof(Type));  // 定义数组,并保存以下局部变量中
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_S, 4);
                else
                    il.Emit(OpCodes.Stloc_3);
                for (int i = 0; i < gtypes.Length; i++) // 这下把TM后面的运行时类型挖出来放到数据组中
                {
                    if (!isvoid) // 加载相关局部变量
                        il.Emit(OpCodes.Ldloc_S, 4);
                    else
                        il.Emit(OpCodes.Ldloc_3);
                    il.Emit(OpCodes.Ldc_I4, i); // 设置索引
                    il.Emit(OpCodes.Ldtoken, gtypes[i]); // 封送类型
                    callmethod = typeof(Type).GetMethod("GetTypeFromHandle"
                        , new Type[] { typeof(RuntimeTypeHandle) });
                    il.Emit(OpCodes.Call, callmethod); // 把运行时类型拖出来
                    il.Emit(OpCodes.Stelem_Ref); // 把类型放进袋子里
                }
            }
            else
            {
                il.Emit(OpCodes.Ldnull); // 把null值封送到以下变量中
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_S, 4);
                else
                    il.Emit(OpCodes.Stloc_3);
            }

            #endregion

            if (types.Length > 0) // 存在方法参数
            {
                il.Emit(OpCodes.Ldc_I4, types.Length);   // 加载数组长度
                il.Emit(OpCodes.Newarr, typeof(object)); // 定义数组,并保存以下局部变量中
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_3);
                else
                    il.Emit(OpCodes.Stloc_2);

                for (int i = 0; i < types.Length; i++)   // 将参数加载到局部变量数组里面
                {
                    if (!isvoid)
                        il.Emit(OpCodes.Ldloc_3);
                    else
                        il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    if (types[i].IsValueType || types[i].IsGenericParameter || types[i].IsGenericType)
                    {
                        // 是值类型,泛型参,泛型拖进来打包装箱
                        il.Emit(OpCodes.Box, GetType(types[i]));
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }

                if (!isvoid) // 加载相关局部变量
                    il.Emit(OpCodes.Ldloc_3);
                else
                    il.Emit(OpCodes.Ldloc_2);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }

            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_0); // 加载method类型变量
            if (!isvoid) // 加载泛型类型数组变量
                il.Emit(OpCodes.Ldloc_S, 4);
            else
                il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Ldloc_1); // 加载参数数组变量
            //callmethod = typeof(Context).GetMethod("CallMethod", new Type[] { typeof(MethodBase), typeof(Type[]), typeof(object[]) });
            //il.Emit(OpCodes.Call, callmethod); // 把方法参数和相关信息扔到一个上下文方法处理
            // calls the method
            if (!method.IsStatic) {
                il.EmitCall(OpCodes.Callvirt, method, null);
            }
            else {
                il.EmitCall(OpCodes.Call, method, null);
            }

            if (!isvoid)
            {
                if (method.ReturnType.IsValueType || method.ReturnType.IsGenericType || method.ReturnType.IsGenericParameter)
                {
                    // 是值类型,泛型参,泛型拖进来脱衣服
                    il.Emit(OpCodes.Unbox_Any, GetType(method.ReturnType));
                }
                else
                {
                    il.Emit(OpCodes.Castclass, method.ReturnType);
                }
                il.Emit(OpCodes.Stloc_2);
                il.Emit(OpCodes.Ldloc_2);
            }
            else
            {
                il.Emit(OpCodes.Pop);
            }
            il.Emit(OpCodes.Ret);

            DynamicInvoker invoker = (DynamicInvoker)dynamicMethod.CreateDelegate(typeof(DynamicInvoker));
            return invoker;
        }

        private static Type GetType(Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        private static Type[] GetParameterTypes(ParameterInfo[] pis)
        {
            Type[] types = new Type[pis.Length];
            for (int i = 0; i < pis.Length; i++) { 
                if(pis[i].ParameterType.IsByRef)
                    types[i] = pis[i].ParameterType.GetElementType();
                else
                    types[i] = pis[i].ParameterType;
            }
            return types;
        }

        #endregion

        #region Assistant function

        private static void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }

        private static object TypeConvert(object source, Type type)
        {
            object obj = System.Convert.ChangeType(source, type);
            return obj;
        }

        #endregion

    }
}
