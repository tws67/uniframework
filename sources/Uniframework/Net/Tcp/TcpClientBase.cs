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
    /// TCP�ͻ��˻���
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
        /// �������ݿ�
        /// </summary>
        /// <param name="dataBlock"></param>
        public virtual void Send(DataBlock dataBlock)
        {
            SafeSend(dataBlock);
        }

        /// <summary>
        /// ��������
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
                sendQueue.Enqueue(target);//��ӵ������б�

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
        /// ��������
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(byte[] data)
        {
            SafeSend(new DataBlock(data));
        }

        #region ���Ժ��ֶ�
        private Socket socket;
        private string host;
        private int port;
        private TSession session;
        private object tag;
        protected bool isConnected;
        protected string serverIP;

        protected Queue sendQueue = new Queue();

        /// <summary>
        /// �������û�����
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// �ͻ����Ƿ��Ѿ��������������������
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

        #region �ܱ����ĳ�Ա
        /// <summary>
        /// ����Tcp���Ӻ������
        /// </summary>
        /// <param name="iar">�첽Socket</param>
        private void ConnectCallBack(IAsyncResult iar)
        {
            try
            {
                socket.EndConnect(iar);
                session = new TSession();

                session.Socket = this.Socket; //���߳���ͬ����Socket
                session.OnReceivedData += new EventHandler<DataBlockArgs>(SessionOnReceivedData);

                OnCreateSession();
                //�������Ӻ�Ӧ��������������
                WaitForData();
                OnConnectServer();
            }
            catch (Exception e)
            {
                //���ӷ�����ʧ��
                OnConnectServerFailed(e);
            }
        }

        /// <summary>
        /// �Ự����������������֮ǰ������
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
        /// �ȴ���������
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
        /// ԭʼͨѶ�����Ѿ�����
        /// </summary>
        protected virtual void OnConnectServer()
        {
            isConnected = true;
            StartHeartBeat();
            OnBuildDataConnection();
        }

        /// <summary>
        /// ���������Ѿ�����
        /// </summary>
        protected virtual void OnBuildDataConnection()
        {
        }

        protected virtual void StartHeartBeat()
        {
            /*���EnableCheckHeartBeat=true,�������������,�����Ͳ��ܵ��û����OnStart()����
             * �������趨һ��������ʱʱ�䣬�ͻ��˼�鳬ʱ��ʱ��Ӧ�����һ�¡��ͻ��˳�����ڸ�ʱ��
             * �Ķ���֮һʱ���ڣ�����һ�����������������᷵��һ���������������ͻ��˾��ܹ�֪�����������ܹ���ȷ����Ӧ��
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
        /// �յ�����
        /// </summary>
        /// <param name="dataBlock">���ݿ�</param>
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
        /// ����������鹦��
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
        /// ֹͣ�����������
        /// </summary>
        protected override void OnStop()
        {
            if (!Disposed && IsConnected)
            {
                base.OnStop();
                isConnected = false;
                Session.Close();

                //todo:���̰߳�ȫ����
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
        /// �ͷ���Դ
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
        /// ��������Ļص�����
        /// </summary>
        /// <param name="o">������δʹ�ã�</param>
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
