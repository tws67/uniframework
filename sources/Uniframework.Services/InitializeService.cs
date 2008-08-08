using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// ϵͳ��ʼ������
    /// </summary>
    public class InitializeService : IInitializeService
    {
        #region IInitializeService Members

        /// <summary>
        /// ��ȡ�û���Ϣ
        /// </summary>
        /// <param name="userName">�û�����</param>
        /// <returns>�û���Ϣ����</returns>
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
