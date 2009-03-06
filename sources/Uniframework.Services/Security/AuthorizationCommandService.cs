using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniframework.Services;
using Uniframework.Db4o;

namespace Uniframework.Security
{
    /// <summary>
    /// 授权操作服务
    /// </summary>
    public class AuthorizationCommandService : IAuthorizationCommandService
    {
        private readonly string DBNAME = "Command";
        private ILogger logger;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCommandService"/> class.
        /// </summary>
        /// <param name="dbService">The db service.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AuthorizationCommandService(IDb4oDatabaseService dbService, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<AuthorizationCommandService>("AuthorizationCommandService");
            try
            {
                db = dbService.Open(DBNAME);
            }
            catch (Exception ex)
            {
                logger.Error("打开授权操作数据库 \"" + DBNAME + "\" 失败" + Environment.NewLine + ex.Message);
                throw ex;
            }
        }

        #region IAuthorizationCommandService Members

        /// <summary>
        /// 保存操作命令
        /// </summary>
        /// <param name="command">命令</param>
        public void Save(AuthorizationCommand command)
        {
            Delete(command);

            // 为操作指定一个唯一的序号
            if (command.Sequence == -1) {
                IList<AuthorizationCommand> lcs = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.Category == command.Category);
                command.Sequence = lcs.Count + 1;
            }
            db.Store(command);
        }

        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="command">命令</param>
        public void Delete(AuthorizationCommand command)
        {
            IList<AuthorizationCommand> lcs = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.Name == command.Name && cmd.CommandUri == cmd.CommandUri);
            foreach (AuthorizationCommand cmd in lcs) {
                db.Delete(cmd); // 删除所有的操作
            }
        }

        /// <summary>
        /// 删除操作命令
        /// </summary>
        /// <param name="commandUri">命令Uri</param>
        public void Delete(string commandUri)
        {
            IList<AuthorizationCommand> lcs = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.CommandUri == commandUri);
            foreach (AuthorizationCommand cmd in lcs) {
                db.Delete(cmd);
            }
        }

        /// <summary>
        /// 清除所有的操作命令
        /// </summary>
        public void Clear()
        {
            IList<AuthorizationCommand> lcs = db.Load<AuthorizationCommand>();
            foreach (AuthorizationCommand cmd in lcs) {
                db.Delete(cmd);
            }
        }

        /// <summary>
        /// 检查指定的授权操作是否存在
        /// </summary>
        /// <param name="command">授权操作</param>
        /// <returns>
        /// 如果存在返回<see cref="true"/>否则为<see cref="false"/>
        /// </returns>
        public bool Exists(AuthorizationCommand command)
        {
            return Exists(command.Name, command.CommandUri);
        }

        /// <summary>
        /// 检查指定名称和操作项的授权操作是否存在于当前服务中
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="commandUri">操作项</param>
        /// <returns>
        /// 如果存在返回<see cref="true"/>否则为<see cref="false"/>
        /// </returns>
        public bool Exists(string name, string commandUri)
        {
            IList<AuthorizationCommand> lcs = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.Name == name && cmd.CommandUri == commandUri);
            return lcs.Count > 0;
        }

        /// <summary>
        /// 获取操作命令
        /// </summary>
        /// <param name="name">命令名称</param>
        /// <param name="commandUri">命令URI.</param>
        /// <returns></returns>
        public AuthorizationCommand Get(string name, string commandUri)
        {
            IList<AuthorizationCommand> lcs = db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.Name == name && cmd.CommandUri == commandUri);
            return lcs.Count > 0 ? lcs[0] : null;
        }

        /// <summary>
        /// 获取命令列表
        /// </summary>
        /// <param name="category">命令分组</param>
        /// <returns>命令列表</returns>
        public IList<AuthorizationCommand> Get(string category)
        {
            return db.Query<AuthorizationCommand>((AuthorizationCommand cmd) => cmd.Category == category);
        }

        /// <summary>
        /// 获取所有命令列表
        /// </summary>
        /// <returns>命令列表</returns>
        public IList<AuthorizationCommand> GetAll()
        {
            return db.Load<AuthorizationCommand>();
        }

        #endregion
    }
}
