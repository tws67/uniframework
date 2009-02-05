using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.Switch
{
    public interface ICTIDriver
    {
        /// <summary>
        /// 名字
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 板卡适配器的版本信息
        /// </summary>
        IVersionInfo VersionInfo { get; }
        /// <summary>
        /// 激活/关闭板卡适配器
        /// </summary>
        bool Active { get; set; }
        /// <summary>
        /// 用于标识当前板卡适配器可否工作
        /// </summary>
        bool CanWork { get; }
        /// <summary>
        /// 超时值
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// 工作项，本地容器
        /// </summary>
        WorkItem WorkItem { get; }
        /// <summary>
        /// 当前适配器初始化的所有通道
        /// </summary>
        ChannelCollection Channels { get; }
        /// <summary>
        /// 通道数
        /// </summary>
        int ChannelCount { get; }
        /// <summary>
        /// 获取指定类型或状态的通道
        /// </summary>
        /// <param name="chnlid">通道标识</param>
        /// <param name="chnlType">通道类型</param>
        /// <param name="chnlStatus">通道状态</param>
        /// <returns>若板卡适配器可用并指定了相应的通道标识则直接返回相应的通道，否则查找指定类型及状态的通道</returns>
        IChannel GetChannel(int chnlid, ChannelType chnlType, ChannelStatus chnlStatus);
        /// <summary>
        /// 加入会议
        /// </summary>
        /// <param name="conf">会议组号</param>
        /// <param name="chnl">语音通道</param>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
        void JoinConf(int conf, IChannel chnl);
        /// <summary>
        /// 离开会议
        /// </summary>
        /// <param name="conf">会议组号</param>
        /// <param name="chnl">语音通道</param>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="ConfCount"/>
        /// <seealso cref="GetConf"/>
        void LeaveConf(int conf, IChannel chnl);
        /// <summary>
        /// 返回系统目前创建的会议组数
        /// </summary>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="GetConf"/>
        int ConfCount { get; }
        /// <summary>
        /// 取得指定会议组的成员列表
        /// </summary>
        /// <param name="confGroup">会议组号</param>
        /// <returns>返回指定组号的会议，如果不存在的话则返回null</returns>
        /// <remarks>Modified By JackyXU 2007-01-12</remarks>
        /// <seealso cref="JoinConf"/>
        /// <seealso cref="LeaveConf"/>
        /// <seealso cref="ConfCount"/>
        List<int> GetConf(int confGroup);

        event EventHandler<EventArgs<IChannel>> CreatedChannel;
    }
}
