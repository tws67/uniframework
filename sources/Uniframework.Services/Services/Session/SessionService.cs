using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Xml;
using Db4objects.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 会话服务
    /// </summary>
    public class SessionService : ISessionService, IDisposable
    {
        private Dictionary<string, SessionState> sessions;
        private IConfigurationService configService;
        private ILogger logger;
        private readonly string THREAD_SOLOT_NAME = "SessionThreadSlot";
        private IEventDispatcher dispatcher;
        private object syncObj = new object();
        private IObjectDatabase db;

        private int timeout;
        private int checkSpan;
        private bool isRunning = true;

        private readonly string SESSION_PAPH = "System/Services/SessionService";
        private readonly string SESSION_DATABASE_NAME = "Session.yap";

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService"/> class.
        /// </summary>
        /// <param name="databaseService">The database service.</param>
        /// <param name="configService">The config service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SessionService(IObjectDatabaseService databaseService, IConfigurationService configService, ILoggerFactory loggerFactory, IEventDispatcher dispatcher)
        {
            //设置Session对象的级联删除
            Db4oFactory.Configure().ObjectClass(typeof(SessionStore)).CascadeOnDelete(true);
            logger = loggerFactory.CreateLogger<SessionService>("Framework");
            db = databaseService.OpenDatabase(SESSION_DATABASE_NAME);
            sessions = new Dictionary<string, SessionState>();

            SessionStore[] stores = db.Load<SessionStore>();
            foreach (SessionStore store in stores)
            {
                sessions.Add(store.Key, store.Session);
                InitialSession(sessions[store.Key]);
            }

            this.configService = configService;
            this.dispatcher = dispatcher;
            try {
                IConfiguration config = new XMLConfiguration(configService.GetItem(SESSION_PAPH));
                timeout = config.Attributes["timeout"] != null ? Convert.ToInt32(config.Attributes["timeout"]) : 180;
                checkSpan = config.Attributes["checkspan"] != null ? Convert.ToInt32(config.Attributes["checkspan"]) : 1000;
            }
            catch (Exception ex) {
                logger.Warn("无法从注册表服务中读取Session服务的设置，采用默认设置", ex);
                timeout = 180; // 3分钟超时
                checkSpan = 1000;
            }

            Thread thread = new Thread(new ThreadStart(delegate
            {
                while (isRunning)
                {
                    CheckingTimeout();
                    Thread.Sleep(checkSpan);
                }
            }));
            thread.Start();
        }

        #region Assistant function 

        /// <summary>
        /// Checkings the timeout.
        /// </summary>
        private void CheckingTimeout()
        {
            lock (sessions)
            {
                List<string> list = new List<string>();
                foreach (string sessionKey in sessions.Keys)
                {
                    if (sessions[sessionKey].IsTimeouted)
                    {
                        logger.Info("会话 [" + sessionKey + "] 超时");
                        list.Add(sessionKey);
                    }
                }
                foreach (string key in list)
                {
                    UnloadSession(key);
                }
            }
        }

        /// <summary>
        /// Initials the session.
        /// </summary>
        /// <param name="session">The session.</param>
        private void InitialSession(SessionState session)
        {
            session.ValueChanged += new EventHandler(session_ValueChanged);
        }

        /// <summary>
        /// Handles the ValueChanged event of the session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void session_ValueChanged(object sender, EventArgs e)
        {
            string id = ((SessionState)sender)[ServerVariables.SESSION_ID].ToString();
            SessionStore store = GetSessionFromDatabase(id);
            store.Session = (SessionState)sender;
            db.Save(store);
        }

        /// <summary>
        /// Gets the session from database.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        /// <returns></returns>
        private SessionStore GetSessionFromDatabase(string sessionID)
        {
            SessionStore[] stores = db.Load<SessionStore>(new Predicate<SessionStore>(
                delegate(SessionStore store)
                {
                    return store.Key == sessionID;
                }));

            if (stores.Length == 0) return null;
            return stores[0];
        }
        #endregion

        #region ISessionService Members

        /// <summary>
        /// Registers the specified session ID.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        public void Register(string sessionID, string userName, string ipAddress, string encryptKey)
        {
            lock (syncObj)
            {
                logger.Info("注册 [" + userName + "]：[" + ipAddress + "] 的会话 [" + sessionID + "]");
                if (sessions.ContainsKey(sessionID))
                    throw new ArgumentException("SessionManager中已经存在Key为 [" + sessionID + "] 的Session");

                SessionState session = new SessionState(timeout, new Hashtable());
                session[ServerVariables.SESSION_ID] = sessionID;
                session[ServerVariables.CURRENT_USER] = userName;
                session[ServerVariables.CLIENT_ADDRESS] = ipAddress;
                session[ServerVariables.ENCRYPT_KEY] = encryptKey;
                session[ServerVariables.LOGGING_TIME] = DateTime.Now;
                sessions.Add(sessionID, session);
                db.Save(new SessionStore(sessionID, session));
                InitialSession(session);
            }
        }

        /// <summary>
        /// Unloads the session.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        public void UnloadSession(string sessionID)
        {
            lock (syncObj)
            {
                if (sessions.ContainsKey(sessionID))
                {
                    SessionStore store = GetSessionFromDatabase(sessionID);
                    db.Delete(store);
                    dispatcher.UnRegisterAllOuterEventSubscriber(sessionID);
                    sessions.Remove(sessionID);
                }
            }
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        /// <returns></returns>
        public ISessionState GetSession(string sessionID)
        {
            if (!sessions.ContainsKey(sessionID)) 
                throw new TimeoutException("SessionManager中不存在ID为 [" + sessionID + "] 的Session。该会话可能已经超时");
            return sessions[sessionID];
        }

        /// <summary>
        /// Activates the specified session ID.
        /// </summary>
        /// <param name="sessionID">The session ID.</param>
        public void Activate(string sessionID)
        {
            if (!sessions.ContainsKey(sessionID)) 
                throw new ArgumentException("SessionManager中不存在ID为 [" + sessionID + "] 的Session。该会话可能已经超时");
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(THREAD_SOLOT_NAME);
            Thread.SetData(slot, sessionID);
            sessions[sessionID].Activate();
        }

        /// <summary>
        /// 当前会话实例
        /// </summary>
        /// <value></value>
        public ISessionState CurrentSession
        {
            get
            {
                LocalDataStoreSlot slot = Thread.GetNamedDataSlot(THREAD_SOLOT_NAME);
                string sessionID = (string)Thread.GetData(slot);
                ISessionState session = GetSession(sessionID);
                return session;
            }
        }

        /// <summary>
        /// 获取所有的会话实例
        /// </summary>
        /// <returns></returns>
        public ISessionState[] GetAllSessions()
        {
            List<ISessionState> list = new List<ISessionState>();
            foreach (ISessionState session in sessions.Values)
            {
                list.Add(session);
            }
            return list.ToArray();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            isRunning = false;
        }

        #endregion
    }

    #region Assistant class

    /// <summary>
    /// 会话存储
    /// </summary>
    class SessionStore
    {
        private string key;
        private SessionState session;

        public SessionStore(string key, SessionState session)
        {
            this.key = key;
            this.session = session;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public SessionState Session
        {
            get
            {
                return session;
            }
            set
            {
                this.session = value;
            }
        }
    }
    #endregion
}
