using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI.EventBroker;
using DevExpress.XtraBars;

namespace Uniframework.SmartClient
{
    public class XtraGridPresenter<TView> : Presenter<TView>
        where TView : IXtraGridView
    {
        private BarManager barManager = null;

        #region Dependency services

        /// <summary>
        /// 系统菜单管理器
        /// </summary>
        /// <value>The bar manager.</value>
        public BarManager BarManager
        {
            get {
                if (barManager == null) {
                    barManager = WorkItem.RootWorkItem.Items.Get<BarManager>(UIExtensionSiteNames.Shell_Bar_Manager);
                }
                return barManager;
            }
        }

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="XtraGridPresenter"/> class.
        /// </summary>
        public XtraGridPresenter()
        {}

        /// <summary>
        /// 初始化视图表格控件
        /// </summary>
        protected virtual void InitXtraGird()
        {
            if (BarManager != null) {
                View.Grid.MenuManager = BarManager;
            }
        }
    }
}
