using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// Wcf通道客户端调用器
    /// </summary>
    public class WcfChannelClient : DuplexClientBase<IInvokeChannel>, IInvokeChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannelClient"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback instance.</param>
        public WcfChannelClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        { }

        #region IInvokeChannel Members

        /// <summary>
        /// 调用远程方法
        /// </summary>
        /// <param name="data">调用数据包</param>
        /// <returns>以字节流返回的调用结果</returns>
        public byte[] Invoke(byte[] data)
        {
            return this.Channel.Invoke(data);
        }

        #endregion
    }
}
