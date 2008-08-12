// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uniframework.Net
{
    /// <summary>
    /// ��������ͻ���֮��ĻỰ��
    /// </summary>
    public class TcpSession : DisposableBase
    { 
        #region �����ֶ�

        /// <summary>
        /// ���������¼���ֻ���ڲ�ʹ��
        /// </summary>
       internal event  EventHandler<DataBlockArgs> OnReceivedData;

        private Socket socket;
        private DataBlock buffer;
        private int id;
        internal IAsyncResult recvResult;
        internal IAsyncResult sendResult;

        /// <summary>
        /// �Ựʹ�õ�Socket
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
            set { 
                socket = value;
                if (value != null)
                    id = value.Handle.ToInt32();
            }
        }

        public DataBlock Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        TimeCounter timer = new TimeCounter();

        public TimeCounter TimeCounter
        {
            get { return timer; }
        } 

        /// <summary>
        /// ���Session�Ƿ��ڻ
        /// </summary>
        /// <param name="timeOut">��ʱʱ��(ms)</param>
        /// <returns>���ڻ����true,���򷵻�false</returns>
        public virtual bool IsActive(int timeOut)
        {
            NetDebuger.PrintDebugMessage(this, string.Format("TimeOut:{0}-Period:{1}", timeOut, TimeCounter.Milliseconds));

            if( timeOut < TimeCounter.Milliseconds)
            {
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public TcpSession()
        {
            OnCreate();
        }

        /// <summary>
        /// Session������ʱ�Զ������á��̳�������޸����Ķ��塣
        /// </summary>
        protected virtual void OnCreate()
        {
            Buffer = new DataBlock(4096);
        }

        /// <summary>
        /// ���յ�readCount�����ݡ�
        /// </summary>
        /// <param name="readCount">���ݶ�ȡ�ĸ���</param>
        internal protected virtual void ReceivedData(int readCount)
        {
            //дָ������
            Buffer.WriteIndex += readCount;

            EventHandler<DataBlockArgs> temp = OnReceivedData;
            if(temp!=null)
            {
                temp(this, new DataBlockArgs(Buffer));
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("TCP-S:{0:0000}", ID);
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close()
        {
            try
            {
                //TODO:������̰߳�ȫ����
                if (Socket != null && !Disposed)
                {
                    lock (Socket) //˫����ⷽ��
                    {
                        if (Socket != null && !Disposed)
                        {
                            if (Socket.Connected) 
                            {
                                /*
                                if (recvResult != null)
                                {
                                    Socket.EndReceive(recvResult);
                                    recvResult = null;
                                }

                                if (sendResult != null)
                                {
                                    Socket.EndSend(sendResult);
                                    sendResult = null;
                                }
                                */
                                Socket.Shutdown(SocketShutdown.Both);
                            }

                            Socket.Close();
                            Socket = null;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            { 
            }
            catch (ObjectDisposedException)
            {
            }
            catch (NetException)
            {
            }
        }

        protected override void Free(bool dispodedByUser)
        {
            if(dispodedByUser)
            {
                Close();
            }

            base.Free(dispodedByUser);
        }
    }
   
}
