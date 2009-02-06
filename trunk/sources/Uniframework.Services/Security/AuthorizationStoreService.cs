using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using Uniframework.Db4o;
using Uniframework.Services;

namespace Uniframework.Security
{
    public class AuthorizationStoreService : IAuthorizationStoreService
    {
        private readonly string AUTHORIZATION_DBNAME = "Authorization.yap";
        private ILogger logger;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStoreService"/> class.
        /// </summary>
        /// <param name="dbService">The db service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AuthorizationStoreService(IDb4oDatabaseService dbService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AuthorizationStoreService>("AuthorizationStoreService");
            try
            {
                db = dbService.Open(AUTHORIZATION_DBNAME);
            }
            catch (Exception ex)
            {
                logger.Error("打开系统授权数据库 \"" + AUTHORIZATION_DBNAME + "\" 失败。" + Environment.NewLine + ex.Message);
            }
        }

        #region IUfAuthorizationSoteService Members

        /// <summary>
        /// 获取指定用户的授权信息
        /// </summary>
        /// <param name="username">用户名称</param>
        /// <returns>返回特定用户的授权信息</returns>
        public IList<AuthorizationStore> GetAuthorizationsByUser(string username)
        {
            string[] roles = Roles.GetRolesForUser(username);
            List<AuthorizationStore> list = new List<AuthorizationStore>();
            foreach (string role in roles) {
                IList<AuthorizationStore> results = db.Load<AuthorizationStore>(delegate(AuthorizationStore authStore) {
                    return authStore.Role == role;
                });
                list.AddRange(results);
            }
            return list;
        }

        /// <summary>
        /// 获取指定角色的授权信息
        /// </summary>
        /// <param name="role">角色名称</param>
        /// <returns>特定角色的授权信息</returns>
        public IList<AuthorizationStore> GetAuthorizationsByRole(string role)
        {
            return db.Load<AuthorizationStore>(delegate(AuthorizationStore authStore) {
                return authStore.Role == role;
            });
        }

        /// <summary>
        /// 保存授权信息
        /// </summary>
        /// <param name="authorizationStore">授权信息</param>
        public void SaveAuthorization(AuthorizationStore authorizationStore)
        {
            DeleteAuthorization(authorizationStore.Role);
            db.Store(authorizationStore);

            // 触发事件处理程序
            if (AuthorizationChanged != null)
                AuthorizationChanged(this, new EventArgs<string>(authorizationStore.Role));
        }

        /// <summary>
        /// 从后台存储中删除指定角色的授权信息
        /// </summary>
        /// <param name="role">The role.</param>
        public void DeleteAuthorization(string role)
        {
            IList<AuthorizationStore> results = db.Load<AuthorizationStore>(delegate(AuthorizationStore authStore) {
                return authStore.Role == role;
            });

            // 从数据库中删除角色的授权信息
            if (results.Count > 0) {
                db.Delete(results[0]);
            }
        }

        public event EventHandler<EventArgs<string>> AuthorizationChanged;

        /// <summary>
        /// 保存授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Save(AuthorizationNode node)
        {
            Delete(node);
            db.Store(node);
        }

        public void Delete(AuthorizationNode node)
        {
            // C# 3.0的查询语法
            IList<AuthorizationNode> results = db.Query((AuthorizationNode authNode) => authNode.AuthorizationUri == node.AuthorizationUri);
            if (results.Count > 0) {
                db.Delete(results[0]); // 删除对象
            }
        }

        /// <summary>
        /// 删除所有的授权节点信息
        /// </summary>
        public void Clear()
        {
            IList<AuthorizationNode> nodes = db.Query<AuthorizationNode>();
            foreach (AuthorizationNode node in nodes) {
                db.Delete(node);
            }
        }

        /// <summary>
        /// 获取系统中所有的授权节点信息
        /// </summary>
        /// <returns>授权节点列表</returns>
        public IList<AuthorizationNode> GetAuthorizationNodes()
        {
            return db.Query<AuthorizationNode>();
        }

        /// <summary>
        /// 保存操作命令
        /// </summary>
        /// <param name="command">命令</param>
        public void SaveCommand(AuthorizationCommand command)
        {
            DeleteCommand(command.CommandUri);
            db.Store(command);
        }

        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="command">命令</param>
        public void DeleteCommand(AuthorizationCommand command)
        {
            DeleteCommand(command.CommandUri);
        }

        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="commandUri">命令Uri</param>
        public void DeleteCommand(string commandUri) 
        {
            IList<AuthorizationCommand> results = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.CommandUri == commandUri);
            foreach (AuthorizationCommand cmd in results) {
                db.Delete(cmd);
            }
        }

        /// <summary>
        /// 清除所有的操作命令
        /// </summary>
        public void ClearCommand()
        {
            IList<AuthorizationCommand> results = db.Query<AuthorizationCommand>();
            foreach (AuthorizationCommand cmd in results) {
                db.Delete(cmd);
            }
        }

        /// <summary>
        /// 获取操作命令
        /// </summary>
        /// <param name="name">命令名称</param>
        /// <param name="commandUri">命令URI.</param>
        /// <returns></returns>
        public AuthorizationCommand GetCommand(string name, string commandUri)
        {
            IList<AuthorizationCommand> results = db.Query<AuthorizationCommand>((AuthorizationCommand command) => command.Name == name &&
                command.CommandUri == commandUri);
            if (results.Count > 0)
                return results[0];
            return null;
        }

        /// <summary>
        /// 获取命令列表
        /// </summary>
        /// <param name="category">命令分组</param>
        /// <returns>命令列表</returns>
        public IList<AuthorizationCommand> GetCommand(string category)
        {
            return db.Query<AuthorizationCommand>((AuthorizationCommand command) => command.Category == category);
        }

        /// <summary>
        /// 获取所有命令列表
        /// </summary>
        /// <returns>命令列表</returns>
        public IList<AuthorizationCommand> GetCommands()
        {
            return db.Query<AuthorizationCommand>();
        }

        #endregion
    }
}
