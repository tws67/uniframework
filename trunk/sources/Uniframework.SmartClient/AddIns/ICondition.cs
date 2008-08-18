using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件条件接口
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 条件动作
        /// </summary>
        ConditionFailedAction Action { get; set; }
        /// <summary>
        /// 检查条件是否有效
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <returns>如果有效返回true，否则返回false</returns>
        bool IsValid(object caller, WorkItem context);
    }

    public enum ConditionFailedAction
    {
        Nothing,
        Exclude,
        Disable
    }
}
