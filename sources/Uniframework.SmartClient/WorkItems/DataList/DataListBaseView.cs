using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.ObjectBuilder;

using Uniframework;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表基类
    /// </summary>
    public partial class DataListBaseView : DevExpress.XtraEditors.XtraUserControl, IDataListHandler
    {
        public DataListBaseView()
        {
            InitializeComponent();
        }

        #region Dependency Services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        #region Method && Properties

        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        public virtual void Initilize() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前可否插入新的数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以插入的话; 否则为, <c>false</c>.</value>
        public virtual bool CanInsert {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public virtual void Insert() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public virtual bool CanEdit { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public virtual void Edit() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public virtual bool CanDelete { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public virtual void Delete() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以展开
        /// </summary>
        /// <value>返回<c>true</c>如果可以展开的话; 否则为, <c>false</c>.</value>
        public virtual bool CanExpand { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// 展开数据列表视图
        /// </summary>
        public virtual void Expand() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以折叠
        /// </summary>
        /// <value>返回<c>true</c>如果可以折叠的话; 否则为, <c>false</c>.</value>
        public virtual bool CanCollaspe { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// 折叠数据列表视图
        /// </summary>
        public virtual void Collaspe() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以刷新
        /// </summary>
        /// <value>返回<c>true</c>如果可以刷新的话; 否则为, <c>false</c>.</value>
        public virtual bool CanRefreshDataSource { 
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public virtual void RefreshDataSource() {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        /// <summary>
        /// 数据插入前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataInserting;
        /// <summary>
        /// 数据插入后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataInserted;

        /// <summary>
        /// 数据编辑前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataEditing;
        /// <summary>
        /// 数据编辑后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataEdited;

        /// <summary>
        /// 数据删除前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataDeleting;
        /// <summary>
        /// 数据删除后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataDeleted;

        /// <summary>
        /// 列表刷新后的事件
        /// </summary>
        public event EventHandler<EventArgs> DataRefreshed;

        #endregion

        /// <summary>
        /// 绑定数据源到列表
        /// </summary>
        protected virtual void BindingDataSource()
        { 

        }
    }
}
