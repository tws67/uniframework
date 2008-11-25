using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 座席自动呼入分配器，用于实现呼入自动排队/呼入自动分配服务；队列分组，分配策略可定制，灵活多样；提供多种ACD 排队方式，如线性算法、
    /// 轮循算法、最长空闲时间优先算法、最少回答时间优先算法、回答次数最少优先算法、呼叫记忆优先算法等
    /// </summary>
    interface IAutoCallDispatcher
    {
        IChannel GetChannel(ChannelType channelType);
    }
}
