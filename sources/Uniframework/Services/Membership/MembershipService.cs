using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MembershipService : IMembershipService
    {
        #region IMembershipService 成员

        /// <summary>
        /// 	<seealso cref="Membership.CreateUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public MembershipUser CreateUser(string username, string password)
        {
            return Membership.CreateUser(username, password);
        }

        /// <summary>
        /// 	<seealso cref="Membership.CreateUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public MembershipUser CreateUser(string username, string password, string email)
        {
            return Membership.CreateUser(username, password, email);
        }

        /// <summary>
        /// 	<seealso cref="Membership.CreateUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, out MembershipCreateStatus status)
        {
            return Membership.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, out status);
        }

        /// <summary>
        /// 	<seealso cref="Membership.DeleteUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DeleteUser(string username)
        {
            return Membership.DeleteUser(username);
        }

        /// <summary>
        /// 	<seealso cref="Membership.GetUser"/>
        /// </summary>
        /// <returns></returns>
        public MembershipUser GetUser()
        {
            return Membership.GetUser();
        }

        /// <summary>
        /// 	<seealso cref="Membership.GetUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public MembershipUser GetUser(string username)
        {
            return Membership.GetUser(username);
        }

        /// <summary>
        /// 	<seealso cref="Membership.FindUserByName"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public MembershipUserCollection FindUserByName(string username)
        {
            return Membership.FindUsersByName(username);
        }

        /// <summary>
        /// 	<seealso cref="Membership.FindUserByEmail"/>
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public MembershipUserCollection FindUserByEmail(string email)
        {
            return Membership.FindUsersByEmail(email);
        }

        /// <summary>
        /// 	<seealso cref="Membership.GetAllUsers"/>
        /// </summary>
        /// <returns></returns>
        public MembershipUserCollection GetAllUsers()
        {
            return Membership.GetAllUsers();
        }

        /// <summary>
        /// 	<seealso cref="Membership.UpdateUser"/>
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(MembershipUser user)
        {
            Membership.UpdateUser(user);
        }

        /// <summary>
        /// 	<seealso cref="Membership.ValidateUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateUser(string username, string password)
        {
            bool success = Membership.ValidateUser(username, password);
            if (success && UserLogoned != null)
                UserLogoned(this, new EventArgs<string>(username));
            return success;
        }

        /// <summary>
        /// 	<seealso cref="MembershipUser.ChangePassword"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            MembershipUser user = Membership.GetUser(username);
            if (user != null)
            {
                return user.ChangePassword(oldPassword, newPassword);
            }
            return false;
        }

        /// <summary>
        /// 	<seealso cref="MembershipUser.GetPassword"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetPassword(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            return user != null ? user.GetPassword() : String.Empty;
        }

        /// <summary>
        /// 	<seealso cref="Roles.CreateRole"/>
        /// </summary>
        /// <param name="rolename"></param>
        public void CreateRole(string rolename)
        {
            Roles.CreateRole(rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.DeleteRole"/>
        /// </summary>
        /// <param name="rolename"></param>
        /// <returns></returns>
        public bool DeleteRole(string rolename)
        {
            return Roles.DeleteRole(rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.RoleExists"/>
        /// </summary>
        /// <param name="rolename"></param>
        /// <returns></returns>
        public bool RoleExists(string rolename)
        {
            return Roles.RoleExists(rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.GetAllRoles"/>
        /// </summary>
        /// <returns></returns>
        public string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        /// <summary>
        /// 	<seealso cref="Roles.GetRolesForUser"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string[] GetRolesForUser(string username)
        {
            return Roles.GetRolesForUser(username);
        }

        /// <summary>
        /// 	<seealso cref="Roles.AddUserToRole"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rolename"></param>
        public void AddUserToRole(string username, string rolename)
        {
            Roles.AddUserToRole(username, rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.AddUsersToRoles"/>
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="rolenames"></param>
        public void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            Roles.AddUsersToRoles(usernames, rolenames);
        }

        /// <summary>
        /// 	<seealso cref="Roles.FindUsesInRole"/>
        /// </summary>
        /// <param name="rolename"></param>
        /// <param name="usernameToMatch"></param>
        /// <returns></returns>
        public string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            return Roles.FindUsersInRole(rolename, usernameToMatch);
        }

        /// <summary>
        /// Determines whether [is user in role] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="rolename">The rolename.</param>
        /// <returns>
        /// 	<c>true</c> if [is user in role] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUserInRole(string username, string rolename)
        {
            return Roles.IsUserInRole(username, rolename);
        }

        /// <summary>
        /// Removes the user from role.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="rolename">The rolename.</param>
        public void RemoveUserFromRole(string username, string rolename)
        {
            Roles.RemoveUserFromRole(username, rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.RemoveUserFromRole"/>
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="rolename"></param>
        public void RemoveUsersFromRole(string[] usernames, string rolename)
        {
            Roles.RemoveUsersFromRole(usernames, rolename);
        }

        /// <summary>
        /// 	<seealso cref="Roles.RemoveUserFromRoles"/>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rolenames"></param>
        public void RemoveUserFromRoles(string username, string[] rolenames)
        {
            Roles.RemoveUserFromRoles(username, rolenames);
        }

        /// <summary>
        /// Chats the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="information">The information.</param>
        public void Chat(string username, string information)
        { 
            if(Chatting != null)
            {
                ChatInfo chatInfo = new ChatInfo();
                chatInfo.UserName = username;
                chatInfo.Information = information;

                Chatting(this, new EventArgs<ChatInfo>(chatInfo));
            }
        }

        #endregion

        #region IMembershipService 事件成员


        /// <summary>
        /// Occurs when [user logoned].
        /// </summary>
        public event EventHandler<EventArgs<string>> UserLogoned;

        /// <summary>
        /// Occurs when [chatting].
        /// </summary>
        public event EventHandler<EventArgs<ChatInfo>> Chatting;

        #endregion
    }
}
