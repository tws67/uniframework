using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniframework.Services;
using Uniframework.Db4o;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权节点服务
    /// </summary>
    public class AuthorizationNodeService : IAuthorizationNodeService
    {
        private readonly string DBNAME = "AuthorizationNode";
        private ILogger logger;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationNodeService"/> class.
        /// </summary>
        /// <param name="dbService">The db service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AuthorizationNodeService(IDb4oDatabaseService dbService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AuthorizationNodeService>("AuthorizationNodeService");
            try
            {
                db = dbService.Open(DBNAME);
            }
            catch (Exception ex)
            {
                logger.Error("打开授权节点数据库 \"" + DBNAME + "\" 失败" + Environment.NewLine + ex.Message);
                throw ex;
            }
        }

        #region IAuthorizationNodeService Members

        /// <summary>
        /// 保存授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Save(AuthorizationNode node)
        {
            Delete(node);
            db.Store(node);
        }

        /// <summary>
        /// 删除授权节点
        /// </summary>
        /// <param name="node">授权节点</param>
        public void Delete(AuthorizationNode node)
        {
            IList<AuthorizationNode> lns = db.Query<AuthorizationNode>((AuthorizationNode an) => an.AuthorizationUri == node.AuthorizationUri);
            foreach (AuthorizationNode an in lns) {
                db.Delete(an); // 删除所有授权节点
            }
        }

        /// <summary>
        /// 删除所有的授权节点信息
        /// </summary>
        public void Clear()
        {
            IList<AuthorizationNode> lns = db.Load<AuthorizationNode>();
            foreach (AuthorizationNode an in lns) {
                db.Delete(an);
            }
        }

        /// <summary>
        /// 获取系统中所有的授权节点信息
        /// </summary>
        /// <returns>授权节点列表</returns>
        public IList<AuthorizationNode> GetAll()
        {
            return db.Load<AuthorizationNode>();
        }

        #endregion
    }
}
