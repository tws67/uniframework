using System;
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
        public const string CMD_COMM_AUTHORIZATION_COMMAND = "/Shell/Module/Foundation/Command/Authorization/Command";
    }
}
