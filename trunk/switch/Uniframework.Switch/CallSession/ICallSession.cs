using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    #region Call session flag

    public enum CallSessionFlag
    { 
        /// <summary>
        /// 无
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 会话已经销毁
        /// </summary>
        RELEASED = 0 << 1
    }
    #endregion

    /// <summary>
    /// 会话接口
    /// </summary>
    public interface ICallSession
    {
        /// <summary>
        /// 会话标识
        /// </summary>
        String CallID { get; }
        /// <summary>
        /// 会话名称
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// 会话标志
        /// </summary>
        CallSessionFlag Sessionflags { get; }
        /// <summary>
        /// 当前会话持有的通道
        /// </summary>
        IChannel Channel { get; }
        /// <summary>
        /// 会话终端接口
        /// </summary>
        //IEndpoint Endpoint { get; }
        /// <summary>
        /// 呼叫配置文件
        /// </summary>
        CallerProfile Profile { get; }
        /// <summary>
        /// 会话事件处理器
        /// </summary>
        StateHandler StateHandler { get; }
    }
}
