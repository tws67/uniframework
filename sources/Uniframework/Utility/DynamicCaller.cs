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
    /// ���ٷ�����ù�����
    /// </summary>
    public static class DynamicCaller
    {
        /// <summary>
        /// ��ȡ��������Ŀ��ٵ�����
        /// </summary>
        /// <param name="methodInfo">������Ϣ</param>
        /// <returns>���ٵ���ί��<see cref="FastInvokeHandler"/></returns>
        public static DynamicInvoker GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), 
                new Type[] { typeof(object), typeof(object[]) }, 
                methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();

            // ��ȡ����������Ϣ
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

                // Ϊÿ���������ɱ��ر���
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    locals[i] = il.DeclareLocal(paramTypes[i], true);
                }
                // ������������Ϣת�浽���ر�����
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldelem_Ref);
                    EmitCastToReference(il, paramTypes[i]);
                    il.Emit(OpCodes.Stloc, locals[i]);
                }

                // ������ջ
                if (!methodInfo.IsStatic)
                {
                    il.Emit(OpCodes.Ldarg_0);
                }

                // �����������ı��ظ�����ջ
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

                // ��������������Ա�֤�������ȷ����
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

                // ����
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
            if (!isvoid) // ���巵��ֵ����
            {
                il.DeclareLocal(GetType(method.ReturnType));
            }
            il.DeclareLocal(typeof(object[]));
            il.DeclareLocal(typeof(Type[]));
            il.Emit(OpCodes.Nop);
            callmethod = typeof(MethodBase).GetMethod("GetCurrentMethod", new Type[0]);
            il.Emit(OpCodes.Call, callmethod);
            il.Emit(OpCodes.Stloc_0); // �ѵ���GetCurrentMethod�ķ���ֵ���͵��ֲ�����0��

            #region AddGenericTypes
            if (method.IsGenericMethod) // ���������Ƿ��ͷ���
            {
                gtypes = method.GetGenericArguments();  // �õ����Ͳ�����
                il.Emit(OpCodes.Ldc_I4, gtypes.Length); // �������鳤��
                il.Emit(OpCodes.Newarr, typeof(Type));  // ��������,���������¾ֲ�������
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_S, 4);
                else
                    il.Emit(OpCodes.Stloc_3);
                for (int i = 0; i < gtypes.Length; i++) // ���°�TM���������ʱ�����ڳ����ŵ���������
                {
                    if (!isvoid) // ������ؾֲ�����
                        il.Emit(OpCodes.Ldloc_S, 4);
                    else
                        il.Emit(OpCodes.Ldloc_3);
                    il.Emit(OpCodes.Ldc_I4, i); // ��������
                    il.Emit(OpCodes.Ldtoken, gtypes[i]); // ��������
                    callmethod = typeof(Type).GetMethod("GetTypeFromHandle"
                        , new Type[] { typeof(RuntimeTypeHandle) });
                    il.Emit(OpCodes.Call, callmethod); // ������ʱ�����ϳ���
                    il.Emit(OpCodes.Stelem_Ref); // �����ͷŽ�������
                }
            }
            else
            {
                il.Emit(OpCodes.Ldnull); // ��nullֵ���͵����±�����
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_S, 4);
                else
                    il.Emit(OpCodes.Stloc_3);
            }

            #endregion

            if (types.Length > 0) // ���ڷ�������
            {
                il.Emit(OpCodes.Ldc_I4, types.Length);   // �������鳤��
                il.Emit(OpCodes.Newarr, typeof(object)); // ��������,���������¾ֲ�������
                if (!isvoid)
                    il.Emit(OpCodes.Stloc_3);
                else
                    il.Emit(OpCodes.Stloc_2);

                for (int i = 0; i < types.Length; i++)   // ���������ص��ֲ�������������
                {
                    if (!isvoid)
                        il.Emit(OpCodes.Ldloc_3);
                    else
                        il.Emit(OpCodes.Ldloc_2);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldarg, i + 1);
                    if (types[i].IsValueType || types[i].IsGenericParameter || types[i].IsGenericType)
                    {
                        // ��ֵ����,���Ͳ�,�����Ͻ������װ��
                        il.Emit(OpCodes.Box, GetType(types[i]));
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }

                if (!isvoid) // ������ؾֲ�����
                    il.Emit(OpCodes.Ldloc_3);
                else
                    il.Emit(OpCodes.Ldloc_2);
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }

            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_0); // ����method���ͱ���
            if (!isvoid) // ���ط��������������
                il.Emit(OpCodes.Ldloc_S, 4);
            else
                il.Emit(OpCodes.Ldloc_3);
            il.Emit(OpCodes.Ldloc_1); // ���ز����������
            //callmethod = typeof(Context).GetMethod("CallMethod", new Type[] { typeof(MethodBase), typeof(Type[]), typeof(object[]) });
            //il.Emit(OpCodes.Call, callmethod); // �ѷ��������������Ϣ�ӵ�һ�������ķ�������
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
                    // ��ֵ����,���Ͳ�,�����Ͻ������·�
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
