using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Uniframework.Services;

namespace Uniframework.Client
{
    /// <summary>
    /// Wcf通道回调处理器
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class CallbackHandler : IWcfCallback
    {
        private SynchronizationContext syncContext = null;

        public void CallBackInvoke(ChatData data)
        {
            //ClientEventDispatcher.Instance.RaiseChatEvent(data);
        }

        /// <summary>
        /// Notifies the data change.
        /// </summary>
        /// <param name="eventResult">The event result.</param>
        public void NotifyDataChange(EventResultData eventResult)
        {
            EventResultData data = eventResult;
            if (data != null) {
                ClientEventDispatcher.Instance.DispachEvent(data.Topic, data.Args);
            }
        }

        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            return true;
        }
    }
}
