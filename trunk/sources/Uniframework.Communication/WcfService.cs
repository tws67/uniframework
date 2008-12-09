using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;

using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// Wcf服务
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed), ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfService : IInvokeChannel, IDisposable
    {
        private static List<IInvokeCallback> callbacks = new List<IInvokeCallback>();
        private IInvokeCallback callback = null;
        private static ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfService"/> class.
        /// </summary>
        public WcfService()
        {
            this.callback = OperationContext.Current.GetCallbackChannel<IInvokeCallback>();
            OperationContext.Current.Channel.Closed += new EventHandler(Channel_Closed);
            OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);
            ICommunicationObject CallBackObject = (ICommunicationObject)callback;

            if (logger == null) {
                logger = DefaultContainer.LoggerFactory.CreateLogger<WcfService>("Framework");
            }

            LocalDataStoreSlot slot = Thread.GetNamedDataSlot("SessionThreadSlot");
            SlotData slotdata = Thread.GetData(slot) as SlotData;
            if (slotdata == null) {
                slotdata = new SlotData();
            }
            slotdata.CallBack = callback;
            Thread.SetData(slot, slotdata);

            if (!callbacks.Contains(callback)) {
                callbacks.Add(callback);
            }
        }

        /// <summary>
        /// Gets the call backs.
        /// </summary>
        /// <value>The call backs.</value>
        public static List<IInvokeCallback> CallBacks
        {
            get
            {
                return callbacks;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (callbacks.Contains(callback)) {
                callbacks.Remove(callback);
                callback = null;
                logger.Debug("一个客户端回调对象及服务对象被销毁");
            }
        }

        #endregion

        #region IInvokeChannel Members

        /// <summary>
        /// 调用服务器端的方法
        /// </summary>
        /// <param name="data">调用数据包</param>
        /// <returns>以字节流返回的调用结果</returns>
        public byte[] Invoke(byte[] data)
        {
            return CommonService.Invoke(data);
        }

        #endregion

        #region Assistant functions

        private void Channel_Closed(object sender, EventArgs e)
        {
            if (callbacks.Contains(callback)) {
                try {
                    callbacks.Remove(callback);
                    logger.Info("Closed Client (CallBack) Removed!");
                }
                catch (Exception ex) {
                    logger.Error("Remove from callbackList.", ex);
                }
            }
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            //logger.Debug("一个服务通道Faulted.");
        }

        #endregion
    }
}