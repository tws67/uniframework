using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

using Uniframework.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 会话服务
    /// </summary>
    public class SessionService : DisposableAndStartableBase, ISessionService
    {
        private readonly string SESSION_SOLOTNAME = "SessionThreadSlot";
        private readonly string SESSION_DB = "Session.yap";
        private readonly string SESSION_PAPH = "System/Services/SessionService";
        private readonly int    SESSION_DEFAULT_TIMEOUT = 180;
        private readonly int    SESSION_DEFAULT_CHECKSPAN = 1000;

        private IEventDispatcher dispatcher;
        private ILogger logger;
        private object syncObj = new object();
        private IDb4oDatabase db;
        private int timeOut;
        private int checkSpan;
        private string dbPath;

        private Dictionary<string, SessionState> sessions = new Dictionary<string, SessionState>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MySessionService"/> class.
        /// </summary>
        /// <param name="dbService">The db service.</param>
        /// <param name="configService">The config service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SessionService(IDb4oDatabaseService dbService, IConfigurationService configService, ILoggerFactory loggerFactory, IEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            logger = loggerFactory.CreateLogger<SessionService>("Framework");
            db = dbService.Open(SESSION_DB);

            // 加载会话数据库中的会话信息
            foreach (SessionState session in db.Load<SessionState>()) {
                sessions.Add(session.SessionId, session);
            }

            try {
                IConfiguration conf = new XMLConfiguration(configService.GetItem(SESSION_PAPH));
                timeOut = conf.Attributes["timeout"] != null ? Int32.Parse(conf.Attributes["timeout"]) : SESSION_DEFAULT_TIMEOUT;
                checkSpan = conf.Attributes["checkspan"] != null ? Int32.Parse(conf.Attributes["checkspan"]) : SESSION_DEFAULT_CHECKSPAN;
            }
            catch {
                timeOut = SESSION_DEFAULT_TIMEOUT;
                checkSpan = SESSION_DEFAULT_CHECKSPAN;
            }
            Start(); // 启动会话管理
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        protected override void OnStart()
        {
            while (IsRun) {
                foreach (SessionState session in sessions.Values) {
                    if (session.IsTimeouted) {
                        logger.Info("会话 [" + session.SessionId + "] 超时");
                        UnloadSession(session.SessionId);
                    }
                }

                Thread.Sleep(checkSpan);
            }
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// This method is called when object is being disposed. Override this method to free resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Free(bool disposing)
        {
            Stop();
        }

        #region ISessionService Members

        /// <summary>
        /// 注册会话
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <param name="user">用户名</param>
        /// <param name="ipAddress">调用方IP地址</param>
        /// <param name="encryptKey">密钥</param>
        public void Register(string sessionId, string user, string ipAddress, string encryptKey)
        {
            lock (syncObj) {
                if (sessions.ContainsKey(sessionId))
                    throw new ArgumentException(String.Format("会话管理器中已经存在 [{0}] 的会话", sessionId));

                logger.Info(String.Format("注册来自 {0} [{1}] 的会话 : {2}", user, ipAddress, sessionId));
                SessionState session = new SessionState(sessionId, timeOut);
                session[SessionVariables.SESSION_ID] = sessionId;
                session[SessionVariables.SESSION_CURRENT_USER] = user;
                session[SessionVariables.SESSION_CLIENT_ADDRESS] = ipAddress;
                session[SessionVariables.SESSION_ENCRYPTKEY] = encryptKey;
                session[SessionVariables.SESSION_LOGIN_TIME] = DateTime.Now;

                // 会话状态上下文变化事件
                session.ContextChanged += new EventHandler(delegate(object sender, EventArgs e) {
                    string id = ((SessionState)sender).SessionId;
                    IList<SessionState> list = db.Load<SessionState>(delegate(SessionState ss) {
                        return ss.SessionId == id;
                    });
                    if (list.Count > 0)
                        db.Delete(list[0]); // 删除原来的会话状态数据
                    db.Store(sender);
                });

                // 将新注册的会话资料保存到数据库
                db.Store(session);
                sessions[sessionId] = session;
            }
        }

        /// <summary>
        /// 注销会话
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        public void UnloadSession(string sessionId)
        {
            lock (sessions) {
                IList<SessionState> list = db.Load<SessionState>(delegate(SessionState ss) {
                    return ss.SessionId == sessionId;
                });
                if (list.Count > 0)
                    db.Delete(list[0]); // 从数据库中删除会话
                dispatcher.UnRegisterAllOuterEventSubscriber(sessionId);
                sessions.Remove(sessionId);
            }
        }

        /// <summary>
        /// 根据会话标识获取会话实例
        /// </summary>
        /// <param name="sessionId">会话标识</param>
        /// <returns>会话实例</returns>
        public ISessionState GetSession(string sessionId)
        {
            if (!sessions.ContainsKey(sessionId))
                throw new TimeoutException(String.Format("不存在 [{0}] 的会话, 该会话可能已经超时.", sessionId));

            return sessions[sessionId];
        }

        /// <summary>
        /// 激活会话
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        public void Activate(string sessionId)
        {
            if (!sessions.ContainsKey(sessionId))
                throw new TimeoutException(String.Format("不存在 [{0}] 的会话, 该会话可能已经超时.", sessionId));

            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(SESSION_SOLOTNAME);
            Thread.SetData(slot, sessionId);
            sessions[sessionId].Activate();
        }

        /// <summary>
        /// 当前会话实例
        /// </summary>
        /// <value></value>
        public ISessionState CurrentSession
        {
            get {
                LocalDataStoreSlot slot = Thread.GetNamedDataSlot(SESSION_SOLOTNAME);
                string sessionId = (string)Thread.GetData(slot);
                ISessionState session = GetSession(sessionId);
                return session;
            } 
        }

        /// <summary>
        /// 获取所有的会话实例
        /// </summary>
        /// <returns></returns>
        public ISessionState[] GetAllSessions()
        {
            IList<SessionState> list = new List<SessionState>();
            foreach (SessionState session in sessions.Values) {
                list.Add(session);
            }
            return list.ToArray();
        }

        #endregion
    }
}
