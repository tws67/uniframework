using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Uniframework.Services;
namespace Uniframework.Client
{
    public class WcfChannel : ICommunicationChannel, IWcfChannel, IDisposable
    {
        // Fields
        private static WcfChannelClient client = null;

        // Methods
        public WcfChannel()
        {
            if (client == null)
            {
                client = new WcfChannelClient(new InstanceContext(new CallbackHandler()));
                client.InnerChannel.Faulted += new EventHandler(this.InnerChannel_Faulted);
                client.ChannelFactory.Closed += new EventHandler(this.ChannelFactory_Closed);
                client.InnerChannel.Closed += new EventHandler(this.InnerChannel_Closed);
                client.InnerChannel.Opened += new EventHandler(this.InnerChannel_Opened);
            }
        }

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

        private void InnerChannel_Closed(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("客户端InnerChannel_Closed。接下来将重建");
            client = null;
        }

        private void InnerChannel_Faulted(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("WCF Inner通道 Faulted.state=" + client.State.ToString());
            if (client.State == CommunicationState.Faulted)
            {
                client.Abort();
            }
            client = null;
        }

        private void InnerChannel_Opened(object sender, EventArgs e)
        {
            ClientEventDispatcher.Instance.Logger.Info("客户端 WCF 通道已经建立。");
        }

        public byte[] Invoke(byte[] data)
        {
            byte[] buffer;
            try
            {
                buffer = client.Invoke(data);
            }
            catch (CommunicationObjectAbortedException exception)
            {
                if (client != null)
                {
                    if (client.State == CommunicationState.Opened)
                    {
                        client.Close();
                    }
                    client = null;
                }
                ClientEventDispatcher.Instance.Logger.Info("捕获CommunicationObjectAbortedException异常,通道将被重建。");
                throw exception;
            }
            catch (FaultException<ExceptionDetail> exception2)
            {
                if (exception2.Detail.InnerException != null)
                {
                    throw new Exception(exception2.Detail.InnerException.Message);
                }
                throw new Exception(exception2.Detail.Message);
            }
            catch (FaultException exception3)
            {
                throw new Exception(exception3.Message);
            }
            catch (Exception exception4)
            {
                throw exception4;
            }
            return buffer;
        }
    }
}
