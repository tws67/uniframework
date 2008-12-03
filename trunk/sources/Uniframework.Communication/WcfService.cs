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
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed), ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfService : IWcfChannel, IDisposable
    {
        // Fields
        private static List<IWcfCallback> callbackList = new List<IWcfCallback>();
        private IWcfCallback callback = null;
        private static ILogger logger;

        // Methods
        public WcfService()
        {
            this.callback = OperationContext.Current.GetCallbackChannel<IWcfCallback>();
            OperationContext.Current.Channel.Closed += new EventHandler(this.Channel_Closed);
            OperationContext.Current.Channel.Faulted += new EventHandler(this.Channel_Faulted);
            ICommunicationObject CallBackObject = (ICommunicationObject)this.callback;
            CallBackObject.Faulted += new EventHandler(this.CallBackObject_Faulted);
            CallBackObject.Closed += new EventHandler(this.CallBackObject_Closed);
            if (logger == null)
            {
                logger = DefaultApplication.LoggerFactory.CreateLogger<WcfService>("Framework");
            }
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot("SessionThreadSlot");
            SlotData slotdata = Thread.GetData(slot) as SlotData;
            if (slotdata == null)
            {
                slotdata = new SlotData();
            }
            slotdata.CallBack = this.callback;
            Thread.SetData(slot, slotdata);
            if (slotdata.SessionID == null)
            {
                logger.Debug(string.Format("为会话'{0}'创建wcf通讯实例。", "未知"));
            }
            else
            {
                logger.Debug(string.Format("为会话'{0}'创建wcf通讯实例。", slotdata.SessionID.ToString()));
            }
            if (!callbackList.Contains(this.callback))
            {
                callbackList.Add(this.callback);
            }
        }

        private void CallBackObject_Closed(object sender, EventArgs e)
        {
            logger.Debug("client closed");
        }

        private void CallBackObject_Faulted(object sender, EventArgs e)
        {
            logger.Debug("client faulted.");
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            logger.Debug("一个服务通道Closed.");
            if (callbackList.Contains(this.callback))
            {
                try
                {
                    callbackList.Remove(this.callback);
                    logger.Info("Closed Client (CallBack) Removed!");
                }
                catch (Exception ex)
                {
                    logger.Error("Remove from callbackList.", ex);
                }
            }
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            logger.Debug("一个服务通道Faulted.");
        }

        public void Dispose()
        {
            if (callbackList.Contains(this.callback))
            {
                callbackList.Remove(this.callback);
                this.callback = null;
                logger.Debug("一个客户端回调对象及服务对象被销毁");
            }
        }

        public byte[] Invoke(byte[] data)
        {
            return CommonService.Invoke(data);
        }

        // Properties
        public static List<IWcfCallback> CallBackList
        {
            get
            {
                return callbackList;
            }
        }
    }

    public class SlotData
    {
        // Fields
        public IWcfCallback CallBack;
        public string SessionID;
    }
}
