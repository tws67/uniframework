using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统初始化服务
    /// </summary>
    public class InitializeService : IInitializeService
    {
        #region IInitializeService Members

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>用户信息对象</returns>
        public UserInfo GetUserInfo(string userName)
        {
            UserInfo userInfo = null;
            MembershipUserCollection users = Membership.FindUsersByName(userName);
            if (users.Count > 0)
            {
                userInfo = new UserInfo(userName, userName);
            }
            return userInfo;
        }

        #endregion
    }
}
