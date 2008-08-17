using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Services;

using Uniframework.Services;

namespace Uniframework.SmartClient
{
    public class AuthorizationService : IAuthorizationService
    {
        private WorkItem workItem;
        private IAuthorizationStoreService authorizationStoreService = null;
        private Dictionary<string, AuthorizationResource> authorizationResources = new Dictionary<string, AuthorizationResource>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        public AuthorizationService([ServiceDependency]WorkItem workItem)
        {
            this.workItem = workItem;
            authorizationStoreService = workItem.Services.Get<IAuthorizationStoreService>();
            if (authorizationStoreService != null)
            {
                List<AuthorizationResource> ars = new List<AuthorizationResource>();
                ars = authorizationStoreService.GetAuthorizationResources(Thread.CurrentPrincipal.Identity.Name);
                foreach (AuthorizationResource ar in ars)
                {
                    authorizationResources[ar.Role] = ar;
                }
            }
        }

        /// <summary>
        /// 系统角色授权信息变化事件订阅器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscriber("TOPIC://Authorization/AuthorizationChanged")]
        public void OnAuthorizationChanged(object sender, EventArgs<string> e)
        {
            if (authorizationResources.ContainsKey(e.Data))
                authorizationResources.Remove(e.Data);
            
            if(authorizationStoreService != null)
            {
                AuthorizationResource ar = authorizationStoreService.GetAuthorizationResource(e.Data);
                authorizationResources[ar.Role] = ar;
            }
        }

        #region IAuthorizationService Members

        /// <summary>
        /// 检查当前用户是否可以执行某项操作
        /// </summary>
        /// <param name="exPath">操作路径</param>
        /// <param name="command">操作对应的命令，目前未使用</param>
        /// <returns>如果基于角色的授权列表中存在此路径表示用户可以执行此操作，否则为不能执行此操作。</returns>
        public bool CanExecute(string command)
        {
            if (authorizationResources.Count == 0)
                return true;
            else {
                bool result = false;
                foreach (AuthorizationResource ar in authorizationResources.Values)
                {
                    result = ar[SecurityUtility.HashObject(command)] == AuthorizationAction.Enable;
                    if (result == true)
                        break;
                }
                return result;
            }
        }

        #endregion
    }
}
