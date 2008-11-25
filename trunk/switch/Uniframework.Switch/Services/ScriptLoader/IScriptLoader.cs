using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 脚本加载工具接口
    /// </summary>
    public interface IScriptLoader
    {
        /// <summary>
        /// 脚本程序文件名
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// 脚本程序主方法入口
        /// </summary>
        string MainPoint { get; set; }
        /// <summary>
        /// 执行脚本程序，简化脚本启动方法主要是为了方便系统通过线程执行
        /// </summary>
        void RunScript();
    }
}
