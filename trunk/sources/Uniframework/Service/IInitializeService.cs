using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 客户端初始化服务
    /// </summary>
    [RemoteService("提供客户端初始化的一些信息", ServiceType.System)]
    public interface IInitializeService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <returns>用户信息对象</returns>
        [ClientCache]
        [RemoteMethod("获取用户信息", true)]
        UserInfo GetUserInfo(string userName);
    }

    #region UserInfo
    /// <summary>
    /// 用户信息类
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
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="displayName">用户显示名</param>
        public UserInfo(string username, string displayName)
            : this()
        {
            this.username = username;
            this.displayName = displayName;
        }

        /// <summary>
        /// 构造函数
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
        /// 获取用户名
        /// </summary>
        public string UserName 
        { 
            get 
            { 
                return username; 
            }
        }

        /// <summary>
        /// 获取用户显示名
        /// </summary>
        public string DispalyName
        { 
            get
            { 
                return displayName;
            } 
        }

        /// <summary>
        /// 备注，由ProfileProvider提供者处理的信息
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        ///// <summary>
        ///// 电子邮件
        ///// </summary>
        //public string Email
        //{
        //    get { return email; }
        //}

        ///// <summary>
        ///// 用户创建时间
        ///// </summary>
        //public string CreateDate
        //{
        //    get { return createDate; }
        //}

        ///// <summary>
        ///// 上次活动时间
        ///// </summary>
        //public string LastActivityDate
        //{
        //    get { return lastActivityDate; }
        //}

        ///// <summary>
        ///// 上次登录时间
        ///// </summary>
        //public string LastLoginDate
        //{
        //    get { return lastLoginDate; }
        //}

        ///// <summary>
        ///// 获取用户拥有的操作
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
