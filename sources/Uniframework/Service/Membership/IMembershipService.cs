using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统管理工作台之成员与角色管理
    /// </summary>
    [RemoteService("IMembershipService", ServiceType.Business, ServiceScope.SmartClient)]
    public interface IMembershipService
    {
        /// <summary>
        /// <seealso cref="Membership.CreateUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/CreateUser")]
        MembershipUser CreateUser(string username, string password);
        /// <summary>
        /// <seealso cref="Membership.CreateUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/CreateUser")]
        MembershipUser CreateUser(string username, string password, string email);
        /// <summary>
        /// <seealso cref="Membership.CreateUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/CreateUser")]
        MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, out MembershipCreateStatus status);
        /// <summary>
        /// <seealso cref="Membership.DeleteUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/DeleteUser")]
        bool DeleteUser(string username);
        /// <summary>
        /// <seealso cref="Membership.GetUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/GetUser")]
        MembershipUser GetUser();
        /// <summary>
        /// <seealso cref="Membership.GetUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/GetUser")]
        MembershipUser GetUser(string username);
        /// <summary>
        /// <seealso cref="Membership.FindUserByName"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/FindUserByName")]
        MembershipUserCollection FindUserByName(string username);
        /// <summary>
        /// <seealso cref="Membership.FindUserByEmail"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/FindUserByEmail")]
        MembershipUserCollection FindUserByEmail(string email);
        /// <summary>
        /// <seealso cref="Membership.GetAllUsers"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/GetAllUsers")]
        MembershipUserCollection GetAllUsers();
        /// <summary>
        /// <seealso cref="Membership.UpdateUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/UpdateUser")]
        void UpdateUser(MembershipUser user);
        /// <summary>
        /// <seealso cref="Membership.ValidateUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/ValidateUser")]
        bool ValidateUser(string username, string password);
        /// <summary>
        /// <seealso cref="MembershipUser.ChangePassword"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/ChangePassword")]
        bool ChangePassword(string username, string oldPassword, string newPassword);
        /// <summary>
        /// <seealso cref="MembershipUser.GetPassword"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [RemoteMethod("FUNC://Membership/GetPassword")]
        string GetPassword(string username);

        /// <summary>
        /// <seealso cref="Roles.CreateRole"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/CreateRole")]
        void CreateRole(string rolename);
        /// <summary>
        /// <seealso cref="Roles.DeleteRole"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/DeleteUser")]
        bool DeleteRole(string rolename);
        /// <summary>
        /// <seealso cref="Roles.RoleExists"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/RoleExists")]
        bool RoleExists(string rolename);
        /// <summary>
        /// <seealso cref="Roles.GetAllRoles"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/GetAllRoles")]
        string[] GetAllRoles();
        /// <summary>
        /// <seealso cref="Roles.GetRolesForUser"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/GetRolesForUser")]
        string[] GetRolesForUser(string username);
        /// <summary>
        /// <seealso cref="Roles.AddUserToRole"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/AddUserToRole")]
        void AddUserToRole(string username, string rolename);
        /// <summary>
        /// <seealso cref="Roles.AddUsersToRoles"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/AddUsersToRoles")]
        void AddUsersToRoles(string[] usernames, string[] rolenames);
        /// <summary>
        /// <seealso cref="Roles.FindUsesInRole"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/FindUsersInRole")]
        string[] FindUsersInRole(string rolename, string usernameToMatch);
        [RemoteMethod("FUNC://Membership/IsUserInRole")]
        /// <summary>
        /// <seealso cref="Roles.IsUserInRole"/>
        /// </summary>
        bool IsUserInRole(string username, string rolename);
        [RemoteMethod("FUNC://Membership/RemoveUserFromRole")]
        /// <summary>
        /// <seealso cref="Roles.RemoveUserFromRole"/>
        /// </summary>
        void RemoveUserFromRole(string username, string rolename);
        /// <summary>
        /// <seealso cref="Roles.RemoveUserFromRole"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/RemoveUsersFromRole")]
        void RemoveUsersFromRole(string[] usernames, string rolename);
        /// <summary>
        /// <seealso cref="Roles.RemoveUserFromRoles"/>
        /// </summary>
        [RemoteMethod("FUNC://Membership/RemoveUserFromRoles")]
        void RemoveUserFromRoles(string username, string[] rolenames);

        [EventPublisher("TOPIC://Membership/Userlogon", EventPublisherScope.Global)]
        event EventHandler<EventArgs<string>> UserLogoned;

        [RemoteMethod("FUNC://Membership/Chat")]
        void Chat(string username, string information);

        [EventPublisher("TOPIC://Membership/Chatting", EventPublisherScope.Global)]
        event EventHandler<EventArgs<ChatInfo>> Chatting;
    }

    #region Assistant classes

    [Serializable]
    public class ChatInfo
    {
        private string username;
        private string information;

        public string UserName
        {
            get { return username; }
            set { username = value; }
        }

        public string Information
        {
            get { return information; }
            set { information = value; }
        }
    }

    #endregion

}
