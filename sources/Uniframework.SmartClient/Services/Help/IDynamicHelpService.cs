using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统动态帮助接口
    /// </summary>
    public interface IDynamicHelpService
    {
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="helpUrl">The help URL.</param>
        void ShowHelp(Assembly assembly, string helpUrl);
    }
}
