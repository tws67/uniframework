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
        private readonly string DBNAME = "Authorization.yap";
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
                db = dbService.Open(DBNAME);
            }
            catch (Exception ex)
            {
                logger.Error("打开系统授权数据库 \"" + DBNAME + "\" 失败。" + Environment.NewLine + ex.Message);
            }
        }

        #region IAuthorizationSoteService Members

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
        public AuthorizationStore GetAuthorizationsByRole(string role)
        {
            IList<AuthorizationStore> results = db.Load<AuthorizationStore>(delegate(AuthorizationStore authStore) {
                return authStore.Role == role;
            });
            return results.Count > 0 ? results[0] : null;
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

        /// <summary>
        /// 返回所有角色的授权信息
        /// </summary>
        /// <returns>授权列表</returns>
        public IList<AuthorizationStore> GetAll()
        {
            try {
                return db.Load<AuthorizationStore>();
            }
            catch {
                return new List<AuthorizationStore>();
            }
        }

        public event EventHandler<EventArgs<string>> AuthorizationChanged;

        #endregion
    }
}
