using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Uniframework.Services;

namespace Uniframework.Client
{
    public class WcfChannelClient : DuplexClientBase<IWcfChannel>, IWcfChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannelClient"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback instance.</param>
        public WcfChannelClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// Invokes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public byte[] Invoke(byte[] data)
        {
            return base.Channel.Invoke(data);
        }

    }
}
