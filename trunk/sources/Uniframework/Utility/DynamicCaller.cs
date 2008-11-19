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

                // Call method
                EmitCall(il, methodInfo);

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
                EmitCall(il, methodInfo);

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

        #region Assistant function

        private static void EmitCall(ILGenerator il, MethodInfo method)
        {
            if ((method.CallingConvention & CallingConventions.VarArgs) != 0)
            {
                if (method.IsFinal || !method.IsVirtual)
                {
                    il.EmitCall(OpCodes.Call, method, null);
                }
                else
                {
                    il.EmitCall(OpCodes.Callvirt, method, null);
                }
            }
            else
            {
                if (method.IsFinal || !method.IsVirtual)
                {
                    il.Emit(OpCodes.Call, method);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, method);
                }
            }
        }

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
