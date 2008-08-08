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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Uniframework.Net
{
    /// <summary>
    /// TCP客户端基类
    /// </summary>
    public class TcpClientBase<TSession> : HeartBeatChecker
        where TSession : TcpSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientBase&lt;TSession&gt;"/> class.
        /// </summary>
        public TcpClientBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpClientBase&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public TcpClientBase(string host, int port)
            : this()
        {
            this.Host = host;
            this.Port = port;
        }

        /// <summary>
        /// 发送数据块
        /// </summary>
        /// <param name="dataBlock"></param>
        public virtual void Send(DataBlock dataBlock)
        {
            SafeSend(dataBlock);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public virtual void Send(byte[] data, int startIndex, int length)
        {
            SafeSend(new DataBlock(data, startIndex, length));
        }

        //private void 
        private void SafeSend(DataBlock target)
        {
            lock (sendQueue.SyncRoot)
            {
                sendQueue.Enqueue(target);//添加到发送列表

                if (sendQueue.Count == 1)
                {
                    AtomSend(target);
                }
            }
        }

        private void AtomSend(DataBlock target)
        {
            try
            {
                Debug.Assert(target != null);

                if (Socket != null)
                    Socket.BeginSend(target.Buffer, target.ReadIndex, target.DataLength, SocketFlags.None,
                                new AsyncCallback(SendEndCallBack), target);
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 100538)
                {
                    RaiseDropLineEvent();
                }
                else
                {
                    RaiseErrorEvent(e);
                }
            }
            catch (Exception e)
            {
                RaiseErrorEvent(e);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(byte[] data)
        {
            SafeSend(new DataBlock(data));
        }

        #region 属性和字段
        private Socket socket;
        private string host;
        private int port;
        private TSession session;
        private object tag;
        protected bool isConnected;
        protected string serverIP;

        protected Queue sendQueue = new Queue();

        /// <summary>
        /// 关联的用户对象
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// 客户端是否已经与服务器建立数据连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>The session.</value>
        public TSession Session
        {
            get { return session; }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// Gets the socket.
        /// </summary>
        /// <value>The socket.</value>
        public Socket Socket
        {
            get { return socket; }
        }

        #endregion

        #region 受保护的成员
        /// <summary>
        /// 建立Tcp连接后处理过程
        /// </summary>
        /// <param name="iar">异步Socket</param>
        private void ConnectCallBack(IAsyncResult iar)
        {
            try
            {
                socket.EndConnect(iar);
                session = new TSession();

                session.Socket = this.Socket; //两者持有同样的Socket
                session.OnReceivedData += new EventHandler<DataBlockArgs>(SessionOnReceivedData);

                OnCreateSession();
                //建立连接后应该立即接收数据
                WaitForData();
                OnConnectServer();
            }
            catch (Exception e)
            {
                //连接服务器失败
                OnConnectServerFailed(e);
            }
        }

        /// <summary>
        /// 会话被创建，接收数据之前被调用
        /// </summary>
        protected virtual void OnCreateSession()
        {
        }

        private void SendEndCallBack(IAsyncResult parameter)
        {
            try
            {
                if (Socket != null)
                {
                    int count = Socket.EndSend(parameter);
                    object data = parameter.AsyncState;
                    lock (sendQueue.SyncRoot)
                    {
                        object head = sendQueue.Dequeue();
                        Debug.Assert(head == data);
                        if (sendQueue.Count != 0)
                        {
                            head = sendQueue.Peek();
                            AtomSend(head as DataBlock);
                        }
                    }

                    OnSendEnd(data as DataBlock);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                RaiseErrorEvent(e);
            }
        }

        protected virtual void OnConnectServerFailed(Exception e)
        {

        }


        protected virtual void RaiseErrorEvent(Exception e)
        {
            OnError(e);
            RaiseDropLineEvent();
        }

        protected virtual void OnError(Exception e)
        {

        }

        /// <summary>
        /// 等待接收数据
        /// </summary>
        private void WaitForData()
        {
            try
            {
                if (Socket != null && Session != null)
                {
                    Socket.BeginReceive(session.Buffer.Buffer,
                        session.Buffer.WriteIndex, session.Buffer.WritableLength,
                        SocketFlags.None, new AsyncCallback(ReceiveDataCallBack), null);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 100537)
                {
                    RaiseDropLineEvent();
                }
                else
                {
                    RaiseErrorEvent(e);
                }
            }
        }

        /// <summary>
        /// 原始通讯连接已经建立
        /// </summary>
        protected virtual void OnConnectServer()
        {
            isConnected = true;
            StartHeartBeat();
            OnBuildDataConnection();
        }

        /// <summary>
        /// 数据连接已经建立
        /// </summary>
        protected virtual void OnBuildDataConnection()
        {
        }

        protected virtual void StartHeartBeat()
        {
            /*如果EnableCheckHeartBeat=true,会启动心跳检查,这样就不能调用基类的OnStart()函数
             * 服务器设定一个心跳超时时间，客户端检查超时的时间应该与此一致。客户端程序会在该时间
             * 的二分之一时间内，发送一个心跳包，服务器会返回一个心跳包，这样客户端就能够知道服务器段能够正确的响应。
             */
            if (EnableCheckHeartBeat)
            {
                checkTimer = new Timer(new TimerCallback(CheckHeartBeatCallBack), null,
                    HeartBeatPeriod / 2, HeartBeatPeriod / 2);
                NetDebuger.PrintDebugMessage("Start heart Beat checker, Period:" + HeartBeatPeriod + "(ms)");
                Session.TimeCounter.Start();
            }
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="dataBlock">数据块</param>
        protected virtual void OnReceivedData(DataBlock dataBlock)
        {
        }

        protected virtual void OnSendEnd(DataBlock target)
        {
        }


        void SessionOnReceivedData(object sender, DataBlockArgs e)
        {
            OnReceivedData(e.DataBlock);
        }

        protected void ReceiveDataCallBack(IAsyncResult iar)
        {
            try
            {
                if (Disposed)
                {
                    return;
                }

                if (Socket == null)
                {
                    RaiseDropLineEvent();
                    return;
                }

                int readCount = 0;

                lock (Socket)
                {
                    readCount = Socket.EndReceive(iar);
                }

                if (0 == readCount)
                {
                    RaiseDropLineEvent();
                }
                else
                {
                    if (Session == null)
                    {
                        return;
                    }

                    lock (Session)
                    {
                        session.ReceivedData(readCount);
                        WaitForData();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                RaiseErrorEvent(e);
            }
        }

        protected virtual void OnDropLine()
        {
        }

        private void RaiseDropLineEvent()
        {
            lock (syncSessionObj)
            {
                bool temp = IsConnected;
                if (temp)
                {
                    Stop();
                    OnDropLine();
                }
            }
        }

        #endregion

        /// <summary>
        /// 启动心跳检查功能
        /// </summary>
        protected override void OnStart()
        {
            IPAddress[] hostIPAddress = Dns.GetHostAddresses(Host);

            if (hostIPAddress.Length == 0)
            {
                throw new NetException("Get host ddress fail");
            }

            isConnected = false;
            sendQueue.Clear();
            IPEndPoint iep = new IPEndPoint(hostIPAddress[0], Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.BeginConnect(iep, ConnectCallBack, null);
            serverIP = hostIPAddress[0].ToString();

            NetDebuger.PrintDebugMessage(string.Format("Connecting server:{0}:{1}...", serverIP.ToString(), port));
        }

        object syncSessionObj = new object();

        /// <summary>
        /// 停止检查心跳功能
        /// </summary>
        protected override void OnStop()
        {
            if (!Disposed && IsConnected)
            {
                base.OnStop();
                isConnected = false;
                Session.Close();

                //todo:多线程安全考虑
                /*
                if (IsRun && Session != null)
                {
                    lock (syncSessionObj)
                    {
                        base.OnStop(); //Stop heart Beater

                        Session.Close();
                        session = null;
                        socket = null;
                        isConnected = false;
                    }
                }*/
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="dispodedByUser"></param>
        protected override void Free(bool dispodedByUser)
        {
            if (dispodedByUser)
            {
                Stop();
            }

            base.Free(dispodedByUser);
        }

        /// <summary>
        /// 检查心跳的回调函数
        /// </summary>
        /// <param name="o">参数（未使用）</param>
        protected override void CheckHeartBeatCallBack(object o)
        {
            if (Session != null)
            {
                if (!Session.IsActive(HeartBeatPeriod))
                {
                    Stop();
                }
            }
        }
    }
}
