using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Uniframework.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统授权存储服务
    /// </summary>
    public class AuthorizationStoreService : IAuthorizationStoreService
    {
        private readonly static string AUTHORIZE_DBNAME = "Authorization.yap";
        private ILogger logger;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationStoreService"/> class.
        /// </summary>
        /// <param name="databaseService">The database service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AuthorizationStoreService(IDb4oDatabaseService databaseService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AuthorizationStoreService>("Framework");
            try
            {
                db = databaseService.Open(AUTHORIZE_DBNAME);
            }
            catch (Exception ex)
            {
                logger.Error("打开系统授权数据库\"" + AUTHORIZE_DBNAME + "\"失败。" + Environment.NewLine + ex.Message);
            }
        }

        #region IAuthorizationStoreService Members

        /// <summary>
        /// 返回指定用户所有的授权资源信息列表
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>根据用户所属的角色返回其拥有的授权资源信息列表</returns>
        public List<AuthorizationResource> GetAuthorizationResources(string username)
        {
            List<AuthorizationResource> list = new List<AuthorizationResource>();
            string[] roles = Roles.GetRolesForUser(username);
            foreach (string role in roles)
            {
                IList<AuthorizationResource> results = db.Load<AuthorizationResource>(delegate(AuthorizationResource ar)
                {
                    return ar.Role == role;
                });
                list.AddRange(results);
            }
            return list;
        }

        /// <summary>
        /// 返回指定角色的授权资源信息
        /// </summary>
        /// <param name="rolename">角色名称</param>
        /// <returns>如果存在指定角色的资源信息则返回，否则返回null</returns>
        public AuthorizationResource GetAuthorizationResource(string rolename)
        {
            IList<AuthorizationResource> results = db.Load<AuthorizationResource>(delegate(AuthorizationResource ar)
            {
                return ar.Role == rolename;
            });
            if (results.Count > 0)
                return results[0];
            return null;
        }

        /// <summary>
        /// 保存授权资源信息
        /// </summary>
        /// <param name="ar">需要保存的授权资源信息</param>
        public void Save(AuthorizationResource ar)
        {
            if (Exists(ar.Role))
                Delete(ar.Role);
            db.Store(ar);
            if (AuthorizationChanged != null)
                AuthorizationChanged(this, new EventArgs<string>(ar.Role));
        }

        /// <summary>
        /// 从存储中删除指定角色的授权资源信息
        /// </summary>
        /// <param name="rolename">角色名称</param>
        public void Delete(string rolename)
        {
            AuthorizationResource ar = GetAuthorizationResource(rolename);
            if (ar != null)
                db.Delete(ar);
        }

        /// <summary>
        /// 系统角色授权信息变化事件，用于及时通知客户端进行更新
        /// </summary>
        public event EventHandler<EventArgs<string>> AuthorizationChanged;

        #endregion

        #region Assistant functions

        private bool Exists(string rolename)
        {
            IList<AuthorizationResource> results = db.Load<AuthorizationResource>(delegate(AuthorizationResource ar)
            {
                return ar.Role == rolename;
            });
            return results.Count > 0;
        }

        #endregion
    }
}
