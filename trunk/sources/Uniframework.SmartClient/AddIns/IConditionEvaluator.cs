using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 条件计算器
    /// </summary>
    public interface IConditionEvaluator
    {
        /// <summary>
        /// 检查条件是否有效
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="condition">条件</param>
        /// <param name="context">上下文</param>
        /// <returns>如果条件有效返回true，否则返回false</returns>
        bool IsValid(object caller, Condition condition, WorkItem context);
    }
}
