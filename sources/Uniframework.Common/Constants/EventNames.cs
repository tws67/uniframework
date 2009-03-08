using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Common
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public static class EventNames
    {
        public const string Membership_CurrentRoleChanged = "topic://Shell/Foundation/Membership/CurrentRoleChanged";
        public const string Membership_CurrentUserChanged = "topic://Shell/Foundation/Membership/CurrentUserChanged";
        public const string Authorization_CurrentCommandChanged = "topic://Shell/Foundation/Membership/CurrentCommandChanged";
    }
}
