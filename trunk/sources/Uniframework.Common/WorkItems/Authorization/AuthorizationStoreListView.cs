using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Practices.ObjectBuilder;
using Uniframework.SmartClient;

namespace Uniframework.Common.WorkItems.Authorization
{
    public partial class AuthorizationStoreListView : DevExpress.XtraEditors.XtraUserControl, IDataListView
    {
        public AuthorizationStoreListView()
        {
            InitializeComponent();
        }

        private AuthorizationStoreListPresenter presenter;
        [CreateNew]
        public AuthorizationStoreListPresenter Presenter
        {
            get { return presenter; }
            set {
                presenter = value;
                presenter.View = this;
            }
        }

        #region IDataListView Members

        /// <summary>
        /// 数据列表处理器
        /// </summary>
        /// <value>The presenter.</value>
        public IDataListHandler DataListHandler
        {
            get { return Presenter as IDataListHandler; }
        }

        /// <summary>
        /// 数据列表视图只读属性
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool ReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
