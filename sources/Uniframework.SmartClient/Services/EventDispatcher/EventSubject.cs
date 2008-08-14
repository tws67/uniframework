using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Uniframework.SmartClient
{
    public abstract class EventSubject
    {
        #region 异步接口动态生成器嵌套类型

        /// <summary>
        /// 异步接口动态生成器
        /// </summary>
        sealed class AsyncObjectGenerator
        {
            #region 私有静态成员

            private static Random random = new Random();
            private static Hashtable assemblies = new Hashtable();
            private static ResolveEventHandler resolveEventHandler = null;
            private static Type delegateType = typeof(Delegate);
            private static MethodInfo getInvocationList = delegateType.GetMethod("GetInvocationList", new Type[] { });
            private static MethodInfo get_Target = delegateType.GetMethod("get_Target", new Type[] { });
            private static MethodInfo combin = delegateType.GetMethod("Combine", new Type[] { typeof(Delegate), typeof(Delegate) });
            private static MethodInfo remove = delegateType.GetMethod("Remove", new Type[] { typeof(Delegate), typeof(Delegate) });
            private static MethodInfo beginInvoke = typeof(System.Windows.Forms.Control).GetMethod("BeginInvoke", new Type[] { typeof(Delegate), typeof(object[]) });

            private static string GetRandomString(int n)
            {
                char[] chars = new char[n];
                for (int i = 0; i < n; i++)
                {
                    chars[i] = GetRandomLetter();
                }
                return new string(chars);
            }

            private static char GetRandomLetter()
            {
                int i = (random.Next() % 26) + 97;
                byte[] b = BitConverter.GetBytes(i);
                return BitConverter.ToChar(b, 0);
            }

            private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                return (Assembly)assemblies[args.Name];
            }

            #endregion

            public AsyncObjectGenerator(Type baseType, Type interfaceType)
            {
                this.baseType = baseType;
                this.Interface = interfaceType;
                methodinfoList = new ArrayList();
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic
                    | BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField;
                foreach (MethodInfo mi in interfaceType.GetMethods(bindingFlags))
                {
                    methodinfoList.Add(mi);
                }
            }

            private Type baseType, Interface;
            private ArrayList methodinfoList;

            #region 生成代码的辅助方法

            private void BuildLocalInteger(ILGenerator gen, int value)
            {
                switch (value)
                {
                    case 0:
                        gen.Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        gen.Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        gen.Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        gen.Emit(OpCodes.Ldc_I4_3);
                        break;
                    case 4:
                        gen.Emit(OpCodes.Ldc_I4_4);
                        break;
                    case 5:
                        gen.Emit(OpCodes.Ldc_I4_5);
                        break;
                    case 6:
                        gen.Emit(OpCodes.Ldc_I4_6);
                        break;
                    case 7:
                        gen.Emit(OpCodes.Ldc_I4_7);
                        break;
                    case 8:
                        gen.Emit(OpCodes.Ldc_I4_8);
                        break;
                    default:
                        gen.Emit(OpCodes.Ldc_I4, value);
                        break;
                }
            }

            private void BuildArgumentInteger(ILGenerator gen, int value)
            {
                switch (value)
                {
                    case 0:
                        gen.Emit(OpCodes.Ldarg_0);
                        break;
                    case 1:
                        gen.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        gen.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        gen.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        if (value < 128)
                        {
                            gen.Emit(OpCodes.Ldarg_S, value);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Ldarg, value);
                        }
                        break;
                }
            }

            #endregion

            private Type BuildDelegate(TypeBuilder type, MethodInfo method, Hashtable ctors, Hashtable invocations)
            {
                TypeBuilder d = type.DefineNestedType(method.Name + "Handler",
                    TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed,
                    typeof(System.MulticastDelegate));
                ParameterInfo[] parameters = method.GetParameters();
                Type[] paramTypes1 = new Type[parameters.Length];
                Type[] paramTypes2 = new Type[parameters.Length + 2];
                for (int i = 0; i < parameters.Length; i++)
                {
                    paramTypes1[i] = parameters[i].ParameterType;
                    paramTypes2[i] = parameters[i].ParameterType;
                }
                paramTypes2[parameters.Length] = typeof(AsyncCallback);
                paramTypes2[parameters.Length + 1] = typeof(object);
                MethodAttributes theAttr = MethodAttributes.Public | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot | MethodAttributes.Virtual;
                MethodImplAttributes theImplAttr = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
                ConstructorBuilder ctor = d.DefineConstructor(MethodAttributes.Public,
                    CallingConventions.Standard, new Type[] { typeof(object), typeof(IntPtr) });
                ctor.SetImplementationFlags(theImplAttr);
                ctors.Add(method.Name, ctor);
                d.DefineMethod("Invoke", theAttr, method.ReturnType, paramTypes1).SetImplementationFlags(theImplAttr);
                MethodBuilder invocation = d.DefineMethod("BeginInvoke", theAttr, typeof(IAsyncResult), paramTypes2);
                invocation.SetImplementationFlags(theImplAttr);
                invocations.Add(method.Name, invocation);
                d.DefineMethod("EndInvoke", theAttr, method.ReturnType,
                    new Type[] { typeof(IAsyncResult) }).SetImplementationFlags(theImplAttr);
                return d;
            }

            public FieldBuilder BuildEvent(TypeBuilder type, MethodInfo method, Type delegateType)
            {
                EventBuilder e = type.DefineEvent("_" + method.Name, EventAttributes.None, delegateType);
                Type[] types = new Type[] { typeof(Delegate), typeof(Delegate) };
                FieldBuilder field = type.DefineField("_" + method.Name, delegateType, FieldAttributes.Private);
                string[] nameprefixes = new string[] { "add", "remove" };
                MethodInfo[] calleds = new MethodInfo[] { combin, remove };
                MethodBuilder[] methods = new MethodBuilder[2];
                for (int i = 0; i < 2; i++)
                {
                    methods[i] = type.DefineMethod(nameprefixes[i] + "__" + method.Name,
                        MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Private,
                        typeof(void), new Type[] { delegateType });
                    ILGenerator gen = methods[i].GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, field);
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Call, calleds[i]);
                    gen.Emit(OpCodes.Castclass, delegateType);
                    gen.Emit(OpCodes.Stfld, field);
                    gen.Emit(OpCodes.Ret);
                }
                e.SetAddOnMethod(methods[0]);
                e.SetRemoveOnMethod(methods[1]);
                return field;
            }

            private Type BuildType(ModuleBuilder module)
            {
                string name = Interface.FullName;
                string ns = string.Empty;
                if (name.IndexOf('.') > 0)
                {
                    int index = name.LastIndexOf('.') + 1;
                    ns = name.Substring(0, index);
                    name = name.Substring(index);
                }
                if (name.StartsWith("I"))
                {
                    name = name.Substring(1);
                }
                MethodAttributes defaultMethodAttr = MethodAttributes.HideBySig | MethodAttributes.Family
                    | MethodAttributes.Virtual | MethodAttributes.ReuseSlot;
                TypeBuilder type = module.DefineType(ns + name + "Object",
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable,
                    this.baseType, new Type[] { Interface });

                #region 定义所有的委托类型

                Hashtable delegateTypes = new Hashtable();
                Hashtable ctors = new Hashtable();
                Hashtable invokes = new Hashtable();
                foreach (MethodInfo mi in methodinfoList)
                {
                    delegateTypes.Add(mi.Name, BuildDelegate(type, mi, ctors, invokes));
                }

                #endregion

                #region 定义默认构造器

                ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public,
                    CallingConventions.Standard, new Type[] { });
                ILGenerator ctorGen = ctor.GetILGenerator();
                ctorGen.Emit(OpCodes.Ldarg_0);
                ctorGen.Emit(OpCodes.Call, typeof(EventSubject).GetConstructor(new Type[] { }));
                ctorGen.Emit(OpCodes.Ret);

                #endregion

                #region 定义所有的事件声明(体现到各自的字段声明中)

                Hashtable delegateFields = new Hashtable();
                foreach (MethodInfo mi in methodinfoList)
                {
                    delegateFields.Add(mi.Name, BuildEvent(type, mi, delegateTypes[mi.Name] as Type));
                }

                #endregion

                #region 实现继承方法AddInvocator

                MethodBuilder addInvocator = type.DefineMethod("AddInvocator", defaultMethodAttr,
                    typeof(void), new Type[] { typeof(object) });
                ILGenerator addInvocatorGen = addInvocator.GetILGenerator();
                LocalBuilder local = addInvocatorGen.DeclareLocal(Interface);
                addInvocatorGen.Emit(OpCodes.Ldarg_1);
                addInvocatorGen.Emit(OpCodes.Isinst, Interface);
                addInvocatorGen.Emit(OpCodes.Stloc, local);
                foreach (MethodInfo mi in methodinfoList)
                {
                    FieldInfo field = delegateFields[mi.Name] as FieldInfo;
                    addInvocatorGen.Emit(OpCodes.Ldarg_0);
                    addInvocatorGen.Emit(OpCodes.Dup);
                    addInvocatorGen.Emit(OpCodes.Ldfld, field);
                    addInvocatorGen.Emit(OpCodes.Ldloc, local);
                    addInvocatorGen.Emit(OpCodes.Dup);
                    addInvocatorGen.Emit(OpCodes.Ldvirtftn, mi);
                    addInvocatorGen.Emit(OpCodes.Newobj, ctors[mi.Name] as ConstructorInfo);
                    addInvocatorGen.Emit(OpCodes.Call, combin);
                    addInvocatorGen.Emit(OpCodes.Castclass, delegateTypes[mi.Name] as Type);
                    addInvocatorGen.Emit(OpCodes.Stfld, field);
                }
                addInvocatorGen.Emit(OpCodes.Ret);

                #endregion

                #region 实现继承方法RemoveInvocator

                MethodBuilder removeInvocator = type.DefineMethod("RemoveInvocator", defaultMethodAttr,
                    typeof(void), new Type[] { typeof(object) });
                ILGenerator removeInvocatorGen = removeInvocator.GetILGenerator();

                Label aexit = removeInvocatorGen.DefineLabel();
                bool isFirst = true;
                foreach (MethodInfo mi in methodinfoList)
                {
                    Type tmpType = delegateTypes[mi.Name] as Type;
                    LocalBuilder local0 = removeInvocatorGen.DeclareLocal(tmpType);
                    LocalBuilder local1 = removeInvocatorGen.DeclareLocal(typeof(int));
                    LocalBuilder local2 = removeInvocatorGen.DeclareLocal(typeof(Delegate[]));
                    Label astart = removeInvocatorGen.DefineLabel();
                    Label anext = removeInvocatorGen.DefineLabel();
                    Label aloop = removeInvocatorGen.DefineLabel();
                    Label aback = removeInvocatorGen.DefineLabel();
                    removeInvocatorGen.Emit(OpCodes.Ldarg_0);
                    removeInvocatorGen.Emit(OpCodes.Ldfld, delegateFields[mi.Name] as FieldInfo);
                    removeInvocatorGen.Emit(OpCodes.Brfalse_S, anext);
                    removeInvocatorGen.Emit(OpCodes.Ldarg_0);
                    removeInvocatorGen.Emit(OpCodes.Ldfld, delegateFields[mi.Name] as FieldInfo);
                    removeInvocatorGen.Emit(OpCodes.Callvirt, getInvocationList);
                    removeInvocatorGen.Emit(OpCodes.Stloc, local2);
                    removeInvocatorGen.Emit(OpCodes.Ldc_I4_0);
                    removeInvocatorGen.Emit(OpCodes.Stloc, local1);
                    removeInvocatorGen.Emit(OpCodes.Br_S, aloop);
                    removeInvocatorGen.MarkLabel(astart);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local2);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local1);
                    removeInvocatorGen.Emit(OpCodes.Ldelem_Ref);
                    removeInvocatorGen.Emit(OpCodes.Castclass, tmpType);
                    removeInvocatorGen.Emit(OpCodes.Stloc, local0);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local0);
                    removeInvocatorGen.Emit(OpCodes.Callvirt, get_Target);
                    removeInvocatorGen.Emit(OpCodes.Ldarg_1);
                    removeInvocatorGen.Emit(OpCodes.Bne_Un_S, aback);
                    removeInvocatorGen.Emit(OpCodes.Ldarg_0);
                    removeInvocatorGen.Emit(OpCodes.Dup);
                    removeInvocatorGen.Emit(OpCodes.Ldfld, delegateFields[mi.Name] as FieldInfo);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local0);
                    removeInvocatorGen.Emit(OpCodes.Call, remove);
                    removeInvocatorGen.Emit(OpCodes.Castclass, tmpType);
                    removeInvocatorGen.Emit(OpCodes.Stfld, delegateFields[mi.Name] as FieldInfo);
                    removeInvocatorGen.MarkLabel(aback);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local1);
                    removeInvocatorGen.Emit(OpCodes.Ldc_I4_1);
                    removeInvocatorGen.Emit(OpCodes.Add);
                    removeInvocatorGen.Emit(OpCodes.Stloc, local1);
                    removeInvocatorGen.MarkLabel(aloop);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local1);
                    removeInvocatorGen.Emit(OpCodes.Ldloc, local2);
                    removeInvocatorGen.Emit(OpCodes.Ldlen);
                    removeInvocatorGen.Emit(OpCodes.Conv_I4);
                    removeInvocatorGen.Emit(OpCodes.Blt_S, astart);
                    removeInvocatorGen.Emit(OpCodes.Br_S, anext);
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        removeInvocatorGen.MarkLabel(aexit);
                        aexit = removeInvocatorGen.DefineLabel();
                    }
                    removeInvocatorGen.Emit(OpCodes.Br_S, aexit);
                    removeInvocatorGen.MarkLabel(anext);
                }
                removeInvocatorGen.MarkLabel(aexit);
                removeInvocatorGen.Emit(OpCodes.Ret);

                #endregion

                #region 实现所有的接口方法

                MethodInfo getEnabled = typeof(EventSubject).GetMethod("GetEnabled",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo checkDelegate = typeof(EventSubject).GetMethod("CheckDelegate",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo checkDelegate2 = typeof(EventSubject).GetMethod("CheckDelegate2",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (MethodInfo mi in methodinfoList)
                {
                    ParameterInfo[] parameters = mi.GetParameters();
                    Type[] types = new Type[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        types[i] = parameters[i].ParameterType;
                    }
                    MethodBuilder method = type.DefineMethod(mi.Name,
                        MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot
                        | MethodAttributes.Private | MethodAttributes.Final, mi.ReturnType, types);
                    type.DefineMethodOverride(method, mi);
                    ILGenerator methodGen = method.GetILGenerator();
                    Type tmpType = delegateTypes[mi.Name] as Type;
                    LocalBuilder local0 = methodGen.DeclareLocal(typeof(Delegate[]));
                    LocalBuilder local1 = methodGen.DeclareLocal(tmpType);
                    LocalBuilder local2 = methodGen.DeclareLocal(typeof(Delegate[]));
                    LocalBuilder local3 = methodGen.DeclareLocal(typeof(int));
                    LocalBuilder local4 = methodGen.DeclareLocal(typeof(object[]));
                    LocalBuilder local5 = methodGen.DeclareLocal(typeof(bool));
                    Label bexit = methodGen.DefineLabel();
                    Label bstart = methodGen.DefineLabel();
                    Label bloop = methodGen.DefineLabel();
                    Label bnext = methodGen.DefineLabel();
                    Label bcall = methodGen.DefineLabel();
                    // 检查event是否为空
                    methodGen.Emit(OpCodes.Ldarg_0);
                    methodGen.Emit(OpCodes.Ldfld, delegateFields[mi.Name] as FieldInfo);
                    methodGen.Emit(OpCodes.Brfalse_S, bexit);
                    // 分解委托者
                    methodGen.Emit(OpCodes.Ldarg_0);
                    methodGen.Emit(OpCodes.Ldfld, delegateFields[mi.Name] as FieldInfo);
                    methodGen.Emit(OpCodes.Callvirt, getInvocationList);
                    methodGen.Emit(OpCodes.Stloc, local0);
                    methodGen.Emit(OpCodes.Ldloc, local0);
                    methodGen.Emit(OpCodes.Ldlen);
                    methodGen.Emit(OpCodes.Conv_I4);
                    methodGen.Emit(OpCodes.Ldc_I4_0);
                    methodGen.Emit(OpCodes.Ble_S, bexit);
                    methodGen.Emit(OpCodes.Ldloc, local0);
                    methodGen.Emit(OpCodes.Stloc, local2);
                    methodGen.Emit(OpCodes.Ldc_I4_0);
                    methodGen.Emit(OpCodes.Stloc, local3);
                    methodGen.Emit(OpCodes.Br_S, bloop);
                    methodGen.MarkLabel(bstart);
                    methodGen.Emit(OpCodes.Ldloc, local2);
                    methodGen.Emit(OpCodes.Ldloc, local3);
                    methodGen.Emit(OpCodes.Ldelem_Ref);
                    methodGen.Emit(OpCodes.Castclass, tmpType);
                    methodGen.Emit(OpCodes.Stloc, local1);

                    // 检查状态开关是否有效
                    methodGen.Emit(OpCodes.Ldarg_0);
                    methodGen.Emit(OpCodes.Ldloc, local1);
                    methodGen.Emit(OpCodes.Callvirt, get_Target);
                    methodGen.Emit(OpCodes.Call, getEnabled);
                    methodGen.Emit(OpCodes.Brfalse_S, bnext);

                    // 由基类处理单一委托
                    methodGen.Emit(OpCodes.Ldarg_0);
                    methodGen.Emit(OpCodes.Ldloc, local1);
                    BuildLocalInteger(methodGen, parameters.Length);
                    methodGen.Emit(OpCodes.Newarr, typeof(object));
                    methodGen.Emit(OpCodes.Stloc, local4);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        methodGen.Emit(OpCodes.Ldloc, local4);
                        BuildLocalInteger(methodGen, i);
                        BuildArgumentInteger(methodGen, i + 1);
                        if (types[i].IsValueType)
                        {
                            methodGen.Emit(OpCodes.Box, types[i]);
                        }
                        methodGen.Emit(OpCodes.Stelem_Ref);
                    }

                    methodGen.Emit(OpCodes.Ldloc, local4);
                    methodGen.Emit(OpCodes.Ldloca, local5);

                    if (mi.ReturnType == typeof(void))
                    {
                        methodGen.Emit(OpCodes.Callvirt, checkDelegate);
                    }
                    else
                    {
                        methodGen.Emit(OpCodes.Callvirt, checkDelegate2);
                    }
                    methodGen.Emit(OpCodes.Ldloc, local5);
                    methodGen.Emit(OpCodes.Brtrue_S, bnext);

                    // 默认异步委托调用
                    methodGen.MarkLabel(bcall);
                    methodGen.Emit(OpCodes.Ldloc, local1);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        BuildArgumentInteger(methodGen, i + 1);
                    }
                    methodGen.Emit(OpCodes.Ldnull);
                    methodGen.Emit(OpCodes.Ldnull);
                    methodGen.Emit(OpCodes.Callvirt, invokes[mi.Name] as MethodInfo);
                    methodGen.Emit(OpCodes.Pop);
                    methodGen.MarkLabel(bnext);
                    methodGen.Emit(OpCodes.Ldloc, local3);
                    methodGen.Emit(OpCodes.Ldc_I4_1);
                    methodGen.Emit(OpCodes.Add);
                    methodGen.Emit(OpCodes.Stloc, local3);
                    methodGen.MarkLabel(bloop);
                    methodGen.Emit(OpCodes.Ldloc, local3);
                    methodGen.Emit(OpCodes.Ldloc, local2);
                    methodGen.Emit(OpCodes.Ldlen);
                    methodGen.Emit(OpCodes.Conv_I4);
                    methodGen.Emit(OpCodes.Blt_S, bstart);
                    methodGen.MarkLabel(bexit);
                    methodGen.Emit(OpCodes.Ret);
                }

                #endregion

                Type returnType = type.CreateType();
                foreach (TypeBuilder builder in delegateTypes.Values)
                {
                    builder.CreateType();
                }
                return returnType;
            }

            /// <summary>
            /// 获取动态异步类型
            /// </summary>
            /// <returns>返回动态类型</returns>
            public Type GetAsyncType()
            {
#if SAVE
				string theFilename = "Kanas.AsyncObject.dll";
#endif
                AssemblyName aname = new AssemblyName();
                aname.Name = string.Format("Kanas.AsyncObject.{0}", GetRandomString(16));
                aname.Version = new Version("1.0.0.0");
                AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(aname,
#if SAVE
					AssemblyBuilderAccess.RunAndSave
#else
 AssemblyBuilderAccess.Run
#endif
);
                ModuleBuilder module = assembly.DefineDynamicModule(GetRandomString(8)
#if SAVE
					, theFilename
#endif
);
                Type t = BuildType(module);
#if SAVE
				assembly.Save(theFilename);
#endif
                assemblies[assembly.FullName] = assembly;
                if (resolveEventHandler == null)
                {
                    resolveEventHandler = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                    AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;
                }
                return t;
            }
        }

        #endregion

        private ArrayList subscripters;
        private bool enabled;
        private object target;

        /// <summary>
        /// 抽象事件分发器构造函数
        /// </summary>
        public EventSubject()
        {
            subscripters = new ArrayList();
            enabled = true;
            target = null;
        }

        #region 通过Reflection.Emit重写的方法

        /// <summary>
        /// 增加委托者
        /// </summary>
        /// <param name="obj">新加入的委托者</param>
        protected abstract void AddInvocator(object obj);

        /// <summary>
        /// 删除委托者
        /// </summary>
        /// <param name="obj">要删除的委托者</param>
        protected abstract void RemoveInvocator(object obj);

        #endregion

        #region 可由子类自定义的方法

        /// <summary>
        /// 检查是否需要向指定对象分发事件
        /// </summary>
        /// <param name="target">订阅者对象</param>
        /// <returns></returns>
        protected virtual bool GetEnabled(object target)
        {
            if (enabled)
            {
                return this.target == null ? true : target == this.target;
            }
            return false;
        }

        /// <summary>
        /// 确定决定如何调用无返回值的委托(同步或异步)
        /// </summary>
        /// <param name="d">委托</param>
        /// <param name="args">委托参数</param>
        /// <param name="handled">返回是否已截获该委托</param>
        protected virtual void CheckDelegate(Delegate d, object[] args, out bool handled)
        {
            handled = false;
        }

        /// <summary>
        /// 确定决定如何调用有返回值的委托(同步或异步)
        /// </summary>
        /// <param name="d">委托</param>
        /// <param name="args">委托参数</param>
        /// <param name="handled">返回是否已截获该委托</param>
        /// <returns>委托的返回值</returns>
        protected virtual object CheckDelegate2(Delegate d, object[] args, out bool handled)
        {
            CheckDelegate(d, args, out handled);
            return null;
        }

        #endregion

        /// <summary>
        /// 委托者数量(在线程中可确定是否还有任何委托者存在)
        /// </summary>
        public int Invocations
        {
            get
            {
                if (enabled)
                {
                    return subscripters.Count;
                }
                return 0;
            }
        }

        #region 向订阅者或客户端程序公开的方法

        /// <summary>
        /// 加入委托者
        /// </summary>
        /// <param name="obj">要加入的委托者</param>
        public void Register(object obj)
        {
            subscripters.Add(obj);
            AddInvocator(obj);
        }

        /// <summary>
        /// 删除委托者
        /// </summary>
        /// <param name="obj">要删除的委托者</param>
        public void UnRegister(object obj)
        {
            subscripters.Remove(obj);
            RemoveInvocator(obj);
        }

        /// <summary>
        /// 暂停执行委托
        /// </summary>
        public void Disable()
        {
            enabled = false;
        }

        /// <summary>
        /// 重启执行委托
        /// </summary>
        public void Enable()
        {
            enabled = true;
            target = null;
        }

        public void Enable(object target)
        {
            enabled = true;
            this.target = target;
        }

        /// <summary>
        /// 获取动态异步对象实例
        /// </summary>
        /// <param name="baseType">基类(必须继承自AsyncObjectBase)</param>
        /// <param name="interfaceType">要异步化的接口</param>
        /// <returns>返回的实例</returns>
        public static EventSubject GetObject(Type baseType, Type interfaceType)
        {
            if (baseType == null || !typeof(EventSubject).IsAssignableFrom(baseType))
            {
                baseType = typeof(EventSubject);
            }
            AsyncObjectGenerator generator = new AsyncObjectGenerator(baseType, interfaceType);
            return Activator.CreateInstance(generator.GetAsyncType()) as EventSubject;
        }

        /// <summary>
        /// 获取动态异步对象实例
        /// </summary>
        /// <param name="interfaceType">要异步化的接口</param>
        /// <returns>返回的实例</returns>
        public static EventSubject GetObject(Type interfaceType)
        {
            return GetObject(null, interfaceType);
        }

        #endregion
    }
}
