using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件单元构建器服务接口
    /// </summary>
    public interface IBuilderService
    {
        /// <summary>
        /// 注册UI适配器工厂
        /// </summary>
        /// <param name="factory">UI适配器工厂</param>
        void RegisterAdapterFactory(IUIElementAdapterFactory factory);
        /// <summary>
        /// 注册UI组件调用命令适配器
        /// </summary>
        /// <param name="invokerType">UI调用者类型</param>
        /// <param name="adapterType">适配器类型</param>
        void RegisterCommandAdapter(Type invokerType, Type adapterType);
        /// <summary>
        /// 注册插件单元构建器
        /// </summary>
        /// <param name="builder">插件单元构建器</param>
        void RegisterBuilder(IBuilder builder);
        /// <summary>
        /// 注销插件单元构建器
        /// </summary>
        /// <param name="className">构建器处理的类型名称</param>
        void UnRegisterBuilder(string className);
        /// <summary>
        /// 获取指定类型的插件单元构建器
        /// </summary>
        /// <param name="className">构建器注册的类型名称</param>
        /// <returns>返回指定类型的构建器，否则抛出构建器未找到的异常</returns>
        IBuilder GetBuilder(string className);
    }
}
