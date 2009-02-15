using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Uniframework.Security;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationStoreListPresenter : DataListPresenter<AuthorizationStoreListView>
    {
        #region Dependency Services

        /// <summary>
        /// Gets or sets the authorization store service.
        /// </summary>
        /// <value>The authorization store service.</value>
        [ServiceDependency]
        public IAuthorizationStoreService AuthorizationStoreService
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 视图准备好方法用于在Presenter中初始化视图
        /// </summary>
        public override void OnViewReady()
        {
            base.OnViewReady();
            Initilize();
        }
        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        public override void Initilize()
        {
            base.Initilize();
            View.LoadAuthorizationsNodes();
        }
    }
}
