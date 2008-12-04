using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Uniframework.Services;
using log4net;
namespace Uniframework.Client
{
    /// <summary>
    /// Wcf通道
    /// </summary>
    public class WcfChannel : ICommunicationChannel, IWcfChannel, IDisposable
    {
        private static WcfChannelClient client = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannel"/> class.
        /// </summary>
        public WcfChannel()
        {
            if (client == null) {
                client = new WcfChannelClient(new InstanceContext(new CallbackHandler()));

                client.ChannelFactory.Closed += new EventHandler(this.ChannelFactory_Closed);
                client.InnerChannel.Faulted += new EventHandler(this.Channel_Faulted);
                client.InnerChannel.Closed += new EventHandler(this.Channel_Closed);
                client.InnerChannel.Opened += new EventHandler(this.Channel_Opened);
            }
        }

        /// <summary>
        /// Invokes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public byte[] Invoke(byte[] data)
        {
            byte[] buffer;
            try {
                buffer = client.Invoke(data);
            }
            catch (CommunicationObjectAbortedException abortedEx) {
                if (client != null) {
                    if (client.State == CommunicationState.Opened) {
                        client.Close();
                    }
                    client = null;
                }
                ClientEventDispatcher.Instance.Logger.Info("捕获CommunicationObjectAbortedException异常,通道将被重建。");
                throw abortedEx;
            }
            catch (FaultException<ExceptionDetail> faultEx) {
                if (faultEx.Detail.InnerException != null) {
                    throw new Exception(faultEx.Detail.InnerException.Message);
                }
                throw new Exception(faultEx.Detail.Message);
            }
            catch (FaultException faultEx) {
                throw new Exception(faultEx.Message);
            }
            catch (Exception ex) {
                throw ex;
            }
            return buffer;
        }

        public void Dispose()
        {
            if (client != null)
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
                client = null;
            }
        }

        #region Assistant functions

        private void ChannelFactory_Closed(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("客户端ChannelFactory_Closed。");
        }

        private void ChannelFactory_Faulted(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("WCF 通道因故障已被重置。state=" + client.State.ToString());
            if (client != null)
            {
                client.Close();
            }
            client = null;
        }

        private void Channel_Closed(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("客户端InnerChannel_Closed。接下来将重建");
            client = null;
        }

        private void Channel_Faulted(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("WCF Inner通道 Faulted.state=" + client.State.ToString());
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
            }
            client = null;
        }

        private void Channel_Opened(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("客户端 WCF 通道已经建立。");
        }

        #endregion
    }
}
