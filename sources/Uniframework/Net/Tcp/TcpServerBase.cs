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
    /// TCP����������
    /// </summary>
    public class TcpServerBase<TSession> : HeartBeatChecker
        where TSession : TcpSession, new()
    {
        #region ���캯��

        /// <summary>
        /// �޲����Ĺ��캯����ʹ��Ĭ������
        /// </summary>
        public TcpServerBase()
            : this(0)//ʹ��0��Ϊ�����˿ڣ���Ҫָ�������˿��������ط�
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="port">�����Ķ˿�</param>
        public TcpServerBase(int port)
            : this(port, 100)//���ػ�����100
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="port">�����Ķ˿�</param>
        /// <param name="capacity">�������������ɵ����ͻ�������</param>
        public TcpServerBase(int port, int capacity)
        {
            Port = port;
            Capacity = capacity;
        }

        #endregion

        #region �ֶ�

        private int port;
        private Socket socket;
        private List<TSession> sessions;

        #endregion

        #region ����

        /// <summary>
        /// �Ự������ʹ��һ���ػ�����ʾһ���ͻ������ӣ�
        /// </summary>
        public int Capacity
        {
            get { return sessions.Capacity; }
            set {
                if (IsRun)
                    throw new TcpException("ָ���µĻػ��������ᵼ�»ػ���������·��䣬������Ҫ�ڷ�����ֹͣ��ʱ��ָ����");

                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                sessions = new List<TSession>(value);
            }
        }

        /// <summary>
        /// ���еĻػ�
        /// </summary>
        public List<TSession> Sessions
        {
            get { return sessions; }
        }

        /// <summary>
        /// �����˿�
        /// </summary>
        public int Port
        {
            get { return port; }
            set {
                if (IsRun)
                    throw new TcpException("��Ҫ�ڷ�����ֹͣ��ʱ�����ָ�������������Ķ˿ڡ�");

                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                port = value;
            }
        }

        /// <summary>
        /// ��ǰ�������Ự��
        /// </summary>
        public int SessionsCount
        {
            get { return Sessions.Count; }
        }

        /// <summary>
        /// ������ʹ�õ��׽���
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
        }

        #endregion

        #region �ܱ����ĳ�Ա����

        /// <summary>
        /// ͨѶ�����¼�
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
        /// �رջỰ���ѻỰ�ӷ��������Ƴ�
        /// </summary>
        /// <param name="session">��Ҫ�رյ�Session</param>
        protected virtual void CloseSession(TSession session)
        {
            lock (Sessions) {
                if (Sessions.Contains(session)) {
                    Sessions.Remove(session);
                    NetDebuger.PrintDebugMessage(session, "Close");
                    NetDebuger.PrintDebugMessage(session, string.Format("Remove:{0}/{1}", SessionsCount, Capacity));
                    OnCloseSession(session); //�ر�ǰ����
                    session.Close();
                }
            }
        }

        /// <summary>
        /// �̳�������޸�����Ķ��壬�������»Ự�����ǵ��á�
        /// ���ﶨ�����ݵĽ��պͿ�ʼ����������
        /// </summary>
        /// <param name="session">���ɵ��»Ự</param>
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
        /// ��������,�߼�������Ҫ���ظú���
        /// </summary>
        /// <param name="session">�Ự</param>
        /// <param name="dataBlock">����</param>
        protected virtual void OnReceivedData(TSession session, DataBlock dataBlock)
        {
        }

        /// <summary>
        /// �ر�Session
        /// </summary>
        /// <param name="session">��Ҫ�رյĻỰ</param>
        protected virtual void OnCloseSession(TSession session)
        {
        }

        /// <summary>
        /// �첽Socket�еĽ��������ӻص�����
        /// </summary>
        /// <param name="parameter"></param>
        private void AcceptCallback(IAsyncResult parameter)
        {
            TSession session = default(TSession);

            try {
                // ����������
                session = CreateSession(socket.EndAccept(parameter));

                if (!Full) {
                    lock (Sessions) {
                        Sessions.Add(session);
                    }

                    // ���ÿͻ������ɺ���������Ƿ�Ϊ�ϸ�Ŀͻ���
                    if (!OnCreateSession(session)) {
                        session.Close();
                        return;
                    }

                    // ��ʼע��ͻ������ݽ����¼�
                    session.OnReceivedData += new EventHandler<DataBlockArgs>(SessionReceivedData);
                    // ��ʼ���տͻ�������
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
                // ������Socket�Ѿ��ر�
            }
            catch (SocketException e)
            {
                HandleSocketException(session, e);

                CloseSession(session); // �������ݷ��ʹ�����Ҫ�رո�Socket
            }
            finally
            {
                WaitForClient();// �������տͻ�������
            }
        }

        /// <summary>
        /// �������Ƿ�����
        /// </summary>
        /// <returns></returns>
        public bool Full
        {
            get {
                return sessions.Count >= sessions.Capacity;
            }
        }

        /// <summary>
        /// �������Ự����
        /// </summary>
        /// <param name="session"></param>
        protected virtual void OnServerFull(TSession session)
        {
        }

        /// <summary>
        /// �����»Ự���������ظ���Ŀ��
        /// </summary>
        /// <param name="remoteSocket"></param>
        /// <returns></returns>
        protected virtual TSession CreateSession(Socket socket)
        {
            TSession session = new TSession();
            session.Socket = socket;
            if (EnableCheckHeartBeat) //Todo:�Ƿ��ڴ�������ʱ��
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
        /// ��ȡ���ݻص�����
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

                session.recvResult = null; // �Ѿ�ʹ�ø��첽״̬
                if (session.Socket != null) {
                    int readCount = session.Socket.EndReceive(parameter);

                    if (readCount == 0) { // Զ���Ѿ��ر�
                        CloseSession(session);
                    }
                    else {
                        session.ReceivedData(readCount);
                        WaitForData(session); // ������������
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // �������ݵ�Socket�Ѿ��ر�
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
                // ��ͨ�쳣����Ҫ�Ͽ��������������
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

        /// ��������
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
        /// ��������
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
            // ��Щ�¼�����Ϊ��Socket�رչ����е������¼�
            if (e.ErrorCode != 10054 && e.ErrorCode != 10053 && e.ErrorCode != 10057 && e.ErrorCode != 10058) {
                ReportError(session, e);
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="session">�������ݵĻػ�</param>
        /// <param name="sendCount">�������ݸ���</param>
        protected virtual void OnSendEnd(TSession session, int sendCount)
        {
        }


        /// <summary>
        /// �����������������ͻ�������
        /// </summary>
        protected override void OnStart()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, Port);
            socket.Bind(iep);

            socket.Listen(100);
            WaitForClient(); //�ȴ��ͻ�������
            NetDebuger.PrintDebugMessage(
                string.Format("{0} is running, listen port:{1} and capacity:{2}", this.GetType().Name, Port, Capacity));
            base.OnStart(); //����������鹦��
        }

        /// <summary>
        /// �ȴ��ͻ�������
        /// </summary>
        private void WaitForClient()
        {
            try {
                socket.BeginAccept(AcceptCallback, null);
            }
            catch (ObjectDisposedException)
            {
                // ������Socket�Ѿ��ر�
            }
        }

        /// <summary>
        /// �������ص�����
        /// </summary>
        /// <param name="para"></param>
        protected override void CheckHeartBeatCallBack(object para)
        {
            List<TSession> closeSessions = new List<TSession>();

            lock (Sessions) {
                foreach (TSession session in Sessions) {
                    if (!session.IsActive(HeartBeatPeriod)) { // todo:�Ƿ���Ҫ�޸ĵ�����������
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
        /// ֹͣ������
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop(); // �ر�������鹦��

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
                //�������׽����Ѿ��ر�
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
