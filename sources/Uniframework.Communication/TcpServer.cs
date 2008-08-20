using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections;
using Uniframework.Services;

namespace Uniframework.Communication
{
    /// <summary>
    /// Tcp连接服务器
    /// </summary>
    public class TcpServer
    {
        private static int DEFAUT_MAX_CONNECTIONS = 100;
        private int port;
        private Socket listener;
        private TcpServiceProvider provider;
        private ArrayList connections;
        private int capacity = DEFAUT_MAX_CONNECTIONS;
        private ILogger logger;

        private AsyncCallback ConnectionReady;
        private WaitCallback AcceptConnection;
        private AsyncCallback ReceivedDataReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="capacity">The capacity.</param>
        /// <param name="logger">The logger.</param>
        public TcpServer(int port, int capacity, ILogger logger)
        {
            provider = new TcpServiceProvider();
            this.port = port;
            this.capacity = capacity;
            this.logger = logger;
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connections = new ArrayList();
            ConnectionReady = new AsyncCallback(ConnectionReady_Handler);
            AcceptConnection = new WaitCallback(AcceptConnection_Handler);
            ReceivedDataReady = new AsyncCallback(ReceivedDataReady_Handler);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(100);
            listener.BeginAccept(ConnectionReady, null);
        }

        /// <summary>
        /// Connections the ready_ handler.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void ConnectionReady_Handler(IAsyncResult ar)
        {
            lock (this)
            {
                if (listener == null) return;
                Socket conn = listener.EndAccept(ar);
                if (connections.Count >= capacity)
                {
                    string msg = "SE001: Server busy";
                    conn.Send(Encoding.UTF8.GetBytes(msg), 0, msg.Length, SocketFlags.None);
                    conn.Shutdown(SocketShutdown.Both);
                    conn.Close();
                }
                else
                {
                    ConnectionState st = new ConnectionState();
                    st.socket = conn;
                    st.server = this;
                    st.provider = (TcpServiceProvider)provider.Clone();
                    st.buffer = new byte[4];
                    connections.Add(st);
                    ThreadPool.QueueUserWorkItem(AcceptConnection, st);
                }
                listener.BeginAccept(ConnectionReady, null);
            }
        }

        /// <summary>
        /// Accepts the connection_ handler.
        /// </summary>
        /// <param name="state">The state.</param>
        private void AcceptConnection_Handler(object state)
        {
            ConnectionState st = state as ConnectionState;
            try { st.provider.OnAcceptConnection(st); }
            catch
            {
            }
            if (st.socket.Connected)
                st.socket.BeginReceive(st.buffer, 0, 0, SocketFlags.None,
                  ReceivedDataReady, st);
        }

        /// <summary>
        /// Receiveds the data ready_ handler.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void ReceivedDataReady_Handler(IAsyncResult ar)
        {
            ConnectionState st = ar.AsyncState as ConnectionState;
            st.socket.EndReceive(ar);
            if (st.socket.Available == 0)
                DropConnection(st);
            else
            {
                try
                {
                    st.provider.OnReceiveData(st);
                }
                catch { }
                if (st.socket.Connected)
                    st.socket.BeginReceive(st.buffer, 0, 0, SocketFlags.None, ReceivedDataReady, st);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                listener.Close();
                listener = null;
                foreach (object obj in connections)
                {
                    ConnectionState st = obj as ConnectionState;
                    try { st.provider.OnDropConnection(st); }
                    catch
                    {
                    }
                    st.socket.Shutdown(SocketShutdown.Both);
                    st.socket.Close();
                }
                connections.Clear();
            }
        }

        /// <summary>
        /// Drops the connection.
        /// </summary>
        /// <param name="st">The st.</param>
        internal void DropConnection(ConnectionState st)
        {
            lock (this)
            {
                st.socket.Shutdown(SocketShutdown.Both);
                st.socket.Close();
                if (connections.Contains(st))
                    connections.Remove(st);
            }
        }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
            }
        }


        /// <summary>
        /// Gets the live connections.
        /// </summary>
        /// <value>The live connections.</value>
        public int AvailableConnections
        {
            get
            {
                lock (this) { return connections.Count; }
            }
        }
    }

    /// <summary>
    /// 连接状态类
    /// </summary>
    public class ConnectionState
    {
        internal Socket socket;
        internal TcpServer server;
        internal TcpServiceProvider provider;
        internal byte[] buffer;

        /// <summary>
        /// Gets the remote end point.
        /// </summary>
        /// <value>The remote end point.</value>
        public EndPoint RemoteEndPoint
        {
            get { return socket.RemoteEndPoint; }
        }

        /// <summary>
        /// Gets the available data.
        /// </summary>
        /// <value>The available data.</value>
        public int AvailableData
        {
            get { return socket.Available; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ConnectionState"/> is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected
        {
            get { return socket.Connected; }
        }

        /// <summary>
        /// Reads the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            return socket.Receive(buffer, offset, count, SocketFlags.None);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            socket.Send(buffer, offset, count, SocketFlags.None);
        }

        /// <summary>
        /// Ends the connection.
        /// </summary>
        public void EndConnection()
        {
            server.DropConnection(this);
        }
    }
}