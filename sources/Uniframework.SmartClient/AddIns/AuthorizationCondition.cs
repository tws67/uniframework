using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统权限/授权管理条件表达式，用于在构建插件元素时进行权限检查
    /// </summary>
    public class AuthorizationCondition : ICondition
    {
        private ConditionFailedAction action = ConditionFailedAction.Disable;
        private string addInPath;
        private string command;

        public AuthorizationCondition(AddInElement element)
        {
            addInPath = element.Path + "/" + element.Id;
            command = element.Command;
        }

        #region ICondition Members

        public string Name
        {
            get { return "AuthorizationCondition"; }
        }

        public ConditionFailedAction Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        /// <summary>
        /// 检查当前用户是否有权限执行插件元素所对应的功能
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="context">执行上下文</param>
        /// <returns>如果通过权限管理系统检查用户可以执行此插件元素对应的功能返回true，否则为false</returns>
        public bool IsValid(object caller, WorkItem context)
        {
            //if (String.IsNullOrEmpty(command))
            //    return true;

            IAuthorizationService authorizationService = context.Services.Get<IAuthorizationService>();
            if (authorizationService != null)
            {
                return authorizationService.CanExecute(command);
            }
            else
                return true;
        }

        #endregion
    }
}
