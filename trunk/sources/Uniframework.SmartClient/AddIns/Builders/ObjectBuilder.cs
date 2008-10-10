using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 通用的对象构建器
    /// </summary>
    public class ObjectBuilder : IBuilder
    {
        #region IBuilder Members

        /// <summary>
        /// 条件处理标志
        /// </summary>
        /// <value></value>
        public bool HandleConditions
        {
            get { return false; }
        }

        /// <summary>
        /// 构建器所处理的类的名称
        /// </summary>
        /// <value></value>
        public string ClassName
        {
            get { return "object"; }
        }

        /// <summary>
        /// 构建插件单元
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="context">上下文，用于存放在构建时需要的组件</param>
        /// <param name="element">插件单元</param>
        /// <param name="subItems">被构建的子对象列表</param>
        /// <returns>构建好的插件单元</returns>
        public object BuildItem(object caller, WorkItem context, AddInElement element, ArrayList subItems)
        {
            object obj = null;
            Type type = Type.GetType(element.Configuration.Attributes["type"]);
            if (type != null)
            {
                try
                {
                    obj = Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }
            return obj;
        }

        #endregion
    }
}
