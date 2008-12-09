using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// Wcf通道
    /// </summary>
    public class WcfChannel : IInvokeChannel, ICommunicationChannel, IDisposable
    {
        private static WcfChannelClient chnl = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfChannel"/> class.
        /// </summary>
        public WcfChannel()
        {
            if (chnl == null) { 
                chnl = new WcfChannelClient(new InstanceContext(new CallbackHandler()));

                chnl.ChannelFactory.Closed +=new EventHandler(ChannelFactory_Closed);
                chnl.ChannelFactory.Faulted +=new EventHandler(ChannelFactory_Faulted);
                chnl.InnerChannel.Opened += new EventHandler(InnerChannel_Opened);
                chnl.InnerChannel.Closed += new EventHandler(InnerChannel_Closed);
                chnl.InnerChannel.Faulted += new EventHandler(InnerChannel_Faulted);
            }
        }

        #region IInvokeChannel Members

        /// <summary>
        /// 调用远程方法
        /// </summary>
        /// <param name="data">调用数据包</param>
        /// <returns>以字节流返回的调用结果</returns>
        public byte[] Invoke(byte[] data)
        {
            byte[] buffer;
            try {
                buffer = chnl.Invoke(data); // 调用远程方法
            }
            catch (CommunicationObjectAbortedException ex) {
                if (chnl != null) {
                    if (chnl.State == CommunicationState.Opened) {
                        chnl.Close();
                    }
                    chnl = null;
                }
                //ClientEventDispatcher.Instance.Logger.Info("捕获CommunicationObjectAbortedException异常,通道将被重建。");
                throw ex;
            }

            catch (FaultException<ExceptionDetail> ex) {
                if (ex.Detail.InnerException != null) {
                    throw new Exception(ex.Detail.InnerException.Message);
                }
                throw new Exception(ex.Detail.Message);
            }

            catch (FaultException ex) {
                throw new Exception(ex.Message);
            }
            catch (Exception ex) {
                throw ex;
            }

            // 返回结果
            return buffer;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (chnl != null && chnl.State == CommunicationState.Opened) {
                chnl.Close();
                chnl = null;
            }
        }

        #endregion

        #region Assistant functions

        private void InnerChannel_Faulted(object sender, EventArgs e)
        {
            //ClientEventDispatcher.Instance.Logger.Info("Wcf 通道失效, 状态为 : " + chnl.State);
            if (chnl.State == CommunicationState.Faulted)
            {
                chnl.Abort();
            }
            chnl = null;
        }

        private void InnerChannel_Closed(object sender, EventArgs e)
        {
            //ClientEventDispatcher.Instance.Logger.Info("客户端Wcf通道被关闭, 系统将自动重建Wcf通道. ");
            chnl = null;
        }

        private void InnerChannel_Opened(object sender, EventArgs e)
        {
            //ClientEventDispatcher.Instance.Logger.Info("客户端Wcf通道已经成功创建");
        }

        private void ChannelFactory_Faulted(object sender, EventArgs e)
        {
            //ClientEventDispatcher.Instance.Logger.Info("客户端Wcf通道因故被重置, 状态为 : " + chnl.State);
            if (chnl != null)
            {
                chnl.Close();
                chnl = null;
            }
        }

        private void ChannelFactory_Closed(object sender, EventArgs e)
        {
            //ClientEventDispatcher.Instance.Logger.Info("客户端通道工厂已经关闭");
        }

        #endregion

    }
}
