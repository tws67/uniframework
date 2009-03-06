﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Common
{
    public static class CommandHandlerNames
    {
        // 公共组件命令
        public const string CMD_COMM_MEMBERSHIP_USER = "/Shell/Module/Foundation/Common/Membership/User";
        public const string CMD_COMM_MEMBERSHIP_SETPASSWORD = "/Shell/Module/Foundation/Common/Membership/User/SetPassword";
        public const string CMD_COMM_MEMBERSHIP_CHANGEPASSWORD = "/Shell/Module/Foundation/Common/Membership/ChangedPassword";

        public const string CMD_COMM_MEMBERSHIP_ROLE = "/Shell/Module/Foundation/Common/Membership/Role";
        public const string CMD_COMM_AUTHORIZATION_COMMAND = "/Shell/Module/Foundation/Common/Authorization/Command";
        public const string CMD_COMM_AUTHORIZATION_STORE = "/Shell/Module/Foundation/Common/Authorization/Store";

        public const string CMD_COMM_AUTHORIZTION_NEWAUTHNODE = "/Shell/Module/Foundation/Common/Authorization/NewAuthNode";
        public const string CMD_COMM_AUTHORIZTION_EDITAUTHNODE = "/Shell/Module/Foundation/Common/Authorization/EditAuthNode";
        public const string CMD_COMM_AUTHORIZATION_DELETEAUTHNODE = "/Shell/Module/Foundation/Common/Authorization/DeleteAuthNode";
        public const string CMD_COMM_AUTHORIZATION_REFRESHAUTHNODE = "/Shell/Module/Foundation/Common/Authorization/RefreshAuthNode";
        public const string CMD_COMM_AUTHORIZATION_SELECTCOMMAND = "/Shell/Module/Foundation/Common/Authorization/AuthNode/SelectCommand";
    }
}