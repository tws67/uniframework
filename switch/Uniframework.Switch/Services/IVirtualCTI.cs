using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.Switch
{
    /// <summary>
    /// Uniframework 虚拟机
    /// </summary>
    public interface IVirtualCTI
    {
        /// <summary>
        /// 虚拟机层次全局变量
        /// </summary>
        Dictionary<String, object> GlobalVars { get; }
        /// <summary>
        /// 工作项 - CAB容器
        /// </summary>
        WorkItem WorkItem { get; }

        /// <summary>
        /// 虚拟机启动时间
        /// </summary>
        DateTime Initialized { get; }
        /// <summary>
        /// 虚拟机会话标识
        /// </summary>
        String Session_ID { get; }
        /// <summary>
        /// 当前虚拟机支持的最大并发会话量
        /// </summary>
        Int32 SessionLimit { get; set; }
        /// <summary>
        /// 虚拟机运行标识
        /// </summary>
        Boolean Running { get; }

        /// <summary>
        /// 启动虚拟机
        /// </summary>
        void Start();
        /// <summary>
        /// 停止虚拟机
        /// </summary>
        void Shutdown();
    }
}
