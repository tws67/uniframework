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
using System.Threading;

namespace Uniframework.Net
{
    /// <summary>
    /// TCP服务器基类
    /// </summary>
    public class TcpServerBase<TSession> : HeartBeatChecker
        where TSession : TcpSession, new()
    {
        #region 构造函数

        /// <summary>
        /// 无参数的构造函数，使用默认设置
        /// </summary>
        public TcpServerBase()
            : this(0)//使用0作为监听端口，需要指定监听端口在其它地方
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">监听的端口</param>
        public TcpServerBase(int port)
            : this(port, 100)//最大回话容量100
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">监听的端口</param>
        /// <param name="capacity">服务器可以容纳的最大客户端容量</param>
        public TcpServerBase(int port, int capacity)
        {
            Port = port;
            Capacity = capacity;
        }

        #endregion

        #region 字段

        private int port;
        private Socket socket;
        private List<TSession> sessions;

        #endregion

        #region 属性

        /// <summary>
        /// 会话容量（使用一个回话来表示一个客户端连接）
        /// </summary>
        public int Capacity
        {
            get { return sessions.Capacity; }
            set {
                if (IsRun)
                    throw new TcpException("指定新的回话容量，会导致回话数组的重新分配，所以需要在服务器停止的时候指定。");

                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                sessions = new List<TSession>(value);
            }
        }

        /// <summary>
        /// 所有的回话
        /// </summary>
        public List<TSession> Sessions
        {
            get { return sessions; }
        }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int Port
        {
            get { return port; }
            set {
                if (IsRun)
                    throw new TcpException("需要在服务器停止的时候才能指定服务器监听的端口。");

                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                port = value;
            }
        }

        /// <summary>
        /// 当前服务器会话数
        /// </summary>
        public int SessionsCount
        {
            get { return Sessions.Count; }
        }

        /// <summary>
        /// 服务器使用的套接字
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
        }

        #endregion

        #region 受保护的成员函数

        /// <summary>
        /// 通讯错误事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="e"></param>
        internal protected virtual void ReportError(TSession session, Exception e)
        {
            if (e is SocketException)
            {
                SocketException se = e as SocketException;
                string msg = string.Format("SocketException Code:{0}, Native Code:{1}", se.ErrorCode, se.NativeErrorCode);
                NetDebuger.PrintErrorMessage(session, msg);
            }

            NetDebuger.PrintErrorMessage(session, e.ToString());
        }

        /// <summary>
        /// 关闭会话，把会话从服务器中移除
        /// </summary>
        /// <param name="session">需要关闭的Session</param>
        protected virtual void CloseSession(TSession session)
        {
            lock (Sessions) {
                if (Sessions.Contains(session)) {
                    Sessions.Remove(session);
                    NetDebuger.PrintDebugMessage(session, "Close");
                    NetDebuger.PrintDebugMessage(session, string.Format("Remove:{0}/{1}", SessionsCount, Capacity));
                    OnCloseSession(session); //关闭前调用
                    session.Close();
                }
            }
        }

        /// <summary>
        /// 继承类可以修改这里的定义，它会在新会话产生是调用。
        /// 这里定义数据的接收和开始接收新数据
        /// </summary>
        /// <param name="session">生成的新会话</param>
        protected virtual bool OnCreateSession(TSession session)
        {
            return true;
        }

        void SessionReceivedData(object sender, DataBlockArgs e)
        {
            TSession session = (TSession)sender;
            OnReceivedData(session, e.DataBlock);
        }

        /// <summary>
        /// 接收数据,逻辑处理需要重载该函数
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="dataBlock">数据</param>
        protected virtual void OnReceivedData(TSession session, DataBlock dataBlock)
        {
        }

        /// <summary>
        /// 关闭Session
        /// </summary>
        /// <param name="session">需要关闭的会话</param>
        protected virtual void OnCloseSession(TSession session)
        {
        }

        /// <summary>
        /// 异步Socket中的接收新连接回调函数
        /// </summary>
        /// <param name="parameter"></param>
        private void AcceptCallback(IAsyncResult parameter)
        {
            TSession session = default(TSession);

            try {
                // 创建新连接
                session = CreateSession(socket.EndAccept(parameter));

                if (!Full) {
                    lock (Sessions) {
                        Sessions.Add(session);
                    }

                    // 调用客户端生成函数，检查是否为合格的客户端
                    if (!OnCreateSession(session)) {
                        session.Close();
                        return;
                    }

                    // 开始注册客户端数据接收事件
                    session.OnReceivedData += new EventHandler<DataBlockArgs>(SessionReceivedData);
                    // 开始接收客户端数据
                    WaitForData(session);

                    NetDebuger.PrintDebugMessage(session, "Create");
                    NetDebuger.PrintDebugMessage(session, string.Format("Add:{0}/{1}", SessionsCount, Capacity));
                }
                else {
                    OnServerFull(session);
                    NetDebuger.PrintDebugMessage(session, "Server full");
                    session.Close();
                }
            }
            catch (ObjectDisposedException)
            {
                // 监听的Socket已经关闭
            }
            catch (SocketException e)
            {
                HandleSocketException(session, e);

                CloseSession(session); // 接收数据发送错误，需要关闭该Socket
            }
            finally
            {
                WaitForClient();// 继续接收客户端连接
            }
        }

        /// <summary>
        /// 服务器是否已满
        /// </summary>
        /// <returns></returns>
        public bool Full
        {
            get {
                return sessions.Count >= sessions.Capacity;
            }
        }

        /// <summary>
        /// 服务器会话满了
        /// </summary>
        /// <param name="session"></param>
        protected virtual void OnServerFull(TSession session)
        {
        }

        /// <summary>
        /// 创建新会话，可以重载该项目。
        /// </summary>
        /// <param name="remoteSocket"></param>
        /// <returns></returns>
        protected virtual TSession CreateSession(Socket socket)
        {
            TSession session = new TSession();
            session.Socket = socket;
            if (EnableCheckHeartBeat) //Todo:是否在此启动定时器
            {
                session.TimeCounter.Start();
            }
            return session;
        }

        protected virtual void WaitForData(TSession session)
        {
            DataBlock Buffer = session.Buffer;
            if (session.Socket != null) {
                session.recvResult = session.Socket.BeginReceive(Buffer.Buffer, Buffer.WriteIndex, Buffer.WritableLength,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), session);
            }
        }

        /// <summary>
        /// 读取数据回调函数
        /// </summary>
        /// <param name="parameter"></param>
        void ReceiveCallback(IAsyncResult parameter)
        {
            TSession session = parameter.AsyncState as TSession;
            try
            {
                if (session.Socket == null) {
                    CloseSession(session);
                }

                session.recvResult = null; // 已经使用该异步状态
                if (session.Socket != null) {
                    int readCount = session.Socket.EndReceive(parameter);

                    if (readCount == 0) { // 远端已经关闭
                        CloseSession(session);
                    }
                    else {
                        session.ReceivedData(readCount);
                        WaitForData(session); // 继续接收数据
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // 接收数据的Socket已经关闭
                CloseSession(session);
            }
            catch (NullReferenceException e)
            {
                ReportError(session, e);
                CloseSession(session);
            }
            catch (SocketException e)
            {
                HandleSocketException(session, e);
                CloseSession(session);
            }
            catch (NetException e)
            {
                CloseSession(session);
                ReportError(session, e);
            }
            catch (Exception e)
            {
                // 普通异常不需要断开与服务器的连接
                ReportError(session, e);
            }
        }

        public virtual void Send(TSession session, DataBlock data)
        {
            AtomSend(session, data.Buffer, data.ReadIndex, data.DataLength);
        }

        public void AtomSend(TSession session, byte[] data, int startIndex, int length)
        {
            try {
                session.sendResult = session.Socket.BeginSend(data, startIndex, length, SocketFlags.None,
                    new AsyncCallback(SendCallback), session);
            }
            catch (ObjectDisposedException)
            {
                CloseSession(session);
            }
            catch (NullReferenceException)
            {
                CloseSession(session);
            }
            catch (SocketException e)
            {
                HandleSocketException(session, e);
                CloseSession(session);
            }
        }

        /// 发送数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public virtual void Send(TSession session, byte[] data, int startIndex, int length)
        {
            AtomSend(session, data, startIndex, length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data"></param>
        public virtual void Send(TSession session, byte[] data)
        {
            AtomSend(session, data, 0, data.Length);
        }

        private void SendCallback(IAsyncResult parameter)
        {
            TSession session = (TSession)parameter.AsyncState;
            try {
                session.sendResult = null;
                OnSendEnd(session, session.Socket.EndSend(parameter));
            }
            catch (ObjectDisposedException)
            {
                CloseSession(session);
            }
            catch (NullReferenceException)
            {
                CloseSession(session);
            }
            catch (SocketException e)
            {
                HandleSocketException(session, e);

                CloseSession(session);
            }
            catch (Exception e)
            {
                ReportError(session, e);
            }
        }

        protected virtual void HandleSocketException(TSession session, SocketException e)
        {
            // 这些事件被认为是Socket关闭过程中的正常事件
            if (e.ErrorCode != 10054 && e.ErrorCode != 10053 && e.ErrorCode != 10057 && e.ErrorCode != 10058) {
                ReportError(session, e);
            }
        }

        /// <summary>
        /// 发送数据完成
        /// </summary>
        /// <param name="session">发送数据的回话</param>
        /// <param name="sendCount">发送数据个数</param>
        protected virtual void OnSendEnd(TSession session, int sendCount)
        {
        }


        /// <summary>
        /// 启动服务器，监听客户端连接
        /// </summary>
        protected override void OnStart()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, Port);
            socket.Bind(iep);

            socket.Listen(100);
            WaitForClient(); //等待客户端连接
            NetDebuger.PrintDebugMessage(
                string.Format("{0} is running, listen port:{1} and capacity:{2}", this.GetType().Name, Port, Capacity));
            base.OnStart(); //启动心跳检查功能
        }

        /// <summary>
        /// 等待客户端连接
        /// </summary>
        private void WaitForClient()
        {
            try {
                socket.BeginAccept(AcceptCallback, null);
            }
            catch (ObjectDisposedException)
            {
                // 服务器Socket已经关闭
            }
        }

        /// <summary>
        /// 心跳检查回调函数
        /// </summary>
        /// <param name="para"></param>
        protected override void CheckHeartBeatCallBack(object para)
        {
            List<TSession> closeSessions = new List<TSession>();

            lock (Sessions) {
                foreach (TSession session in Sessions) {
                    if (!session.IsActive(HeartBeatPeriod)) { // todo:是否需要修改到服务器方法
                        closeSessions.Add(session);
                        NetDebuger.PrintDebugMessage(session, "Heartbeat is timeout and add it to closing list");
                    }
                }
            }

            foreach (TSession session in closeSessions) {
                CloseSession(session);
            }
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop(); // 关闭心跳检查功能

            lock (Sessions) {
                TSession[] array = Sessions.ToArray();

                foreach (TSession session in array) {
                    CloseSession(session);
                }

                Sessions.Clear();
            }

            try {
                Socket.Close();
            }
            catch (ObjectDisposedException)
            {
                //服务器套接字已经关闭
            }

            NetDebuger.PrintDebugMessage(this.GetType().Name + " Stop, clear resource success");
        }

        protected override void Free(bool dispodedByUser)
        {
            Stop();
            base.Free(dispodedByUser);
        }

        #endregion
    }
}
