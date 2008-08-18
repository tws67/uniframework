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
        /// Builds the item.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        /// <param name="subItems">The sub items.</param>
        /// <returns></returns>
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
