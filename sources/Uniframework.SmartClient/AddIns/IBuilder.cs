using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件单元构建器
    /// </summary>
    public interface IBuilder
    {
        /// <summary>
        /// 条件处理标志
        /// </summary>
        bool HandleConditions { get; }
        /// <summary>
        /// 构建器所处理的类的名称
        /// </summary>
        string ClassName { get; }
        /// <summary>
        /// 构建插件单元
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="context">上下文，用于存放在构建时需要的组件</param>
        /// <param name="element">插件单元</param>
        /// <param name="subItems">被构建的子对象列表</param>
        /// <returns>构建好的插件单元</returns>
        object BuildItem(object caller, WorkItem context, AddInElement element, ArrayList subItems);
    }
}
