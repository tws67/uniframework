using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// �ͻ��˳�ʼ������
    /// </summary>
    [RemoteService("�ṩ�ͻ��˳�ʼ����һЩ��Ϣ", ServiceType.System)]
    public interface IInitializeService
    {
        /// <summary>
        /// ��ȡ�û���Ϣ
        /// </summary>
        /// <param name="userName">�û�����</param>
        /// <returns>�û���Ϣ����</returns>
        [ClientCache]
        [RemoteMethod("��ȡ�û���Ϣ", true)]
        UserInfo GetUserInfo(string userName);
    }

    #region UserInfo
    /// <summary>
    /// �û���Ϣ��
    /// </summary>
    [Serializable]
    public class UserInfo
    {
        string username = string.Empty;
        string displayName = string.Empty;
        string comment = string.Empty;
        //string email = string.Empty;
        //string createDate = string.Empty;
        //string lastActivityDate = string.Empty;
        //string lastLoginDate = string.Empty;
        //ActionAttribute[] actions = null;

        /// <summary>
        /// 
        /// </summary>
        public UserInfo()
        { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="displayName">�û���ʾ��</param>
        public UserInfo(string username, string displayName)
            : this()
        {
            this.username = username;
            this.displayName = displayName;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="username"></param>
        /// <param name="displayName"></param>
        /// <param name="comment"></param>
        public UserInfo(string username, string displayName, string comment)
            : this(username, displayName)
        {
            this.comment = comment;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="displayName"></param>
        ///// <param name="comment"></param>
        ///// <param name="actions"></param>
        //public UserInfo(string username, string displayName, string comment, ActionAttribute[] actions)
        //    : this(username, displayName, actions)
        //{
        //    this.comment = comment;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="dispName"></param>
        ///// <param name="comment"></param>
        ///// <param name="email"></param>
        ///// <param name="createDate"></param>
        ///// <param name="lastActivityDate"></param>
        ///// <param name="lastLoginDate"></param>
        ///// <param name="actions"></param>
        //public UserInfo(string username, string dispName, string comment, string email, string createDate, string lastActivityDate, string lastLoginDate, ActionAttribute[] actions)
        //    : this(username, dispName, comment, actions)
        //{
        //    this.email = email;
        //    this.createDate = createDate;
        //    this.lastActivityDate = lastActivityDate;
        //    this.lastLoginDate = lastLoginDate;
        //}

        #region UserInfo Members

        /// <summary>
        /// ��ȡ�û���
        /// </summary>
        public string UserName 
        { 
            get 
            { 
                return username; 
            }
        }

        /// <summary>
        /// ��ȡ�û���ʾ��
        /// </summary>
        public string DispalyName
        { 
            get
            { 
                return displayName;
            } 
        }

        /// <summary>
        /// ��ע����ProfileProvider�ṩ�ߴ������Ϣ
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        ///// <summary>
        ///// �����ʼ�
        ///// </summary>
        //public string Email
        //{
        //    get { return email; }
        //}

        ///// <summary>
        ///// �û�����ʱ��
        ///// </summary>
        //public string CreateDate
        //{
        //    get { return createDate; }
        //}

        ///// <summary>
        ///// �ϴλʱ��
        ///// </summary>
        //public string LastActivityDate
        //{
        //    get { return lastActivityDate; }
        //}

        ///// <summary>
        ///// �ϴε�¼ʱ��
        ///// </summary>
        //public string LastLoginDate
        //{
        //    get { return lastLoginDate; }
        //}

        ///// <summary>
        ///// ��ȡ�û�ӵ�еĲ���
        ///// </summary>
        //public ActionAttribute[] Actions
        //{ 
        //    get
        //    {
        //        return actions;
        //    }
        //}

        #endregion
    }

#endregion

}
