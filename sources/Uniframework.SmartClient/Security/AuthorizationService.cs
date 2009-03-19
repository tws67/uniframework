using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Services;

using Uniframework.Services;
using Uniframework.Security;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统权限验证服务
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private WorkItem workItem;
        private IAuthorizationStoreService authorizationStoreService = null;
        private IList<AuthorizationStore> authorizations = new List<AuthorizationStore>();
        private object syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        public AuthorizationService([ServiceDependency]WorkItem workItem)
        {
            this.workItem = workItem;
            authorizationStoreService = workItem.Services.Get<IAuthorizationStoreService>();
            if (authorizationStoreService != null) {
                authorizations = authorizationStoreService.GetAuthorizationsByUser(Thread.CurrentPrincipal.Identity.Name); // 获取当前用户的所有授权信息
            }
        }

        /// <summary>
        /// 系统角色授权信息变化事件订阅器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [EventSubscriber("topic://Authorization/AuthorizationChanged")]
        public void OnAuthorizationChanged(object sender, EventArgs<string> e)
        {
            lock (syncObj) {
                authorizations.Clear();
                if (authorizationStoreService == null) {
                    authorizationStoreService = workItem.Services.Get<IAuthorizationStoreService>();
                    if (authorizationStoreService != null)
                        authorizations = authorizationStoreService.GetAuthorizationsByUser(Thread.CurrentPrincipal.Identity.Name);
                }
            }
        }

        #region IAuthorizationService Members

        /// <summary>
        /// 检查当前用户是否可以执行某项操作
        /// </summary>
        /// <param name="exPath">操作路径</param>
        /// <param name="command">操作对应的命令，目前未使用</param>
        /// <returns>如果基于角色的授权列表中存在此路径表示用户可以执行此操作，否则为不能执行此操作。</returns>
        public bool CanExecute(string authorizationUri)
        {
            lock (syncObj) {
                bool flag = false;
                foreach (AuthorizationStore store in authorizations) {
                    flag |= store.CanExecute(authorizationUri); // 取并集
                }
                return flag;
            }
        }

        #endregion
    }
}
