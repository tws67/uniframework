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
    /// 服务器与客户端之间的会话类
    /// </summary>
    public class TcpSession : DisposableBase
    { 
        #region 属性字段

        /// <summary>
        /// 接收数据事件，只在内部使用
        /// </summary>
       internal event  EventHandler<DataBlockArgs> OnReceivedData;

        private Socket socket;
        private DataBlock buffer;
        private int id;
        internal IAsyncResult recvResult;
        internal IAsyncResult sendResult;

        /// <summary>
        /// 会话使用的Socket
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
        /// 网络活动计时器
        /// </summary>
        TimeCounter timer = new TimeCounter();

        public TimeCounter TimeCounter
        {
            get { return timer; }
        } 

        /// <summary>
        /// 检查Session是否还在活动
        /// </summary>
        /// <param name="timeOut">超时时间(ms)</param>
        /// <returns>正在活动返回true,否则返回false</returns>
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
        /// 默认构造函数
        /// </summary>
        public TcpSession()
        {
            OnCreate();
        }

        /// <summary>
        /// Session被创建时自动被调用。继承类可以修改它的定义。
        /// </summary>
        protected virtual void OnCreate()
        {
            Buffer = new DataBlock(4096);
        }

        /// <summary>
        /// 接收到readCount个数据。
        /// </summary>
        /// <param name="readCount">数据读取的个数</param>
        internal protected virtual void ReceivedData(int readCount)
        {
            //写指针增加
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
                //TODO:解决多线程安全问题
                if (Socket != null && !Disposed)
                {
                    lock (Socket) //双链检测方法
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
