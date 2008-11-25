using System;
using System.Collections.Generic;
using System.Text;

using log4net;
using Microsoft.Practices.CompositeUI;
using Uniframework;

namespace Uniframework.Switch
{
    /// <summary>
    /// 通道包装器，用于事件订阅者对通道事件的响应
    /// </summary>
    public class ChannelWrapper : AbstractChannel
    {
        public ChannelWrapper(WorkItem workItem, ICTIDriver driver, int channelID)
            : base(driver, channelID)
        { 
        }

        public new void OnChannelStatusChanged(object sender, EventArgs<ChannelStatus> e)
        {
            base.OnChannelStatusChanged(sender, e);
        }

        public new void OnGetDTMF(object sender, EventArgs<string> e)
        {
            base.OnGetDTMF(sender, e);
        }

        public new void OnCall(object sender, CallEventArgs e)
        {
            base.OnCall(sender, e);
        }

        public new void OnLinkedToChannel(object sender, EventArgs<IChannel> e)
        {
            base.OnLinkedToChannel(sender, e);
        }

        public new void OnResetedChannel(object sender, EventArgs e)
        {
            base.OnResetedChannel(sender, e);
        }

        public new void OnProcessTimeout(object sender, EventArgs e)
        {
            base.OnProcessTimeout(sender, e);
        }
    }
}
