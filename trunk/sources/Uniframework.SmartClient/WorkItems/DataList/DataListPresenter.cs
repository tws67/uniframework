using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI.Common;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表视图控制器，通过此控制器的继承类来实现对数据列表的添加、编辑、删除等功能
    /// </summary>
    /// <typeparam name="TView">数据列表视图模板类，数据视图必须实现<seealso cref="IDataListView"/>接口</typeparam>
    public class DataListPresenter<TView> : DataPresenter<TView> , IDataListHandler
        where TView : IDataListView
    {
        #region IDataListHandler Members

        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        public virtual void Initilize()
        {
        }

        /// <summary>
        /// 获取一个值决定当前可否插入新的数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以插入的话; 否则为, <c>false</c>.</value>
        public virtual bool CanInsert
        {
            get { return true; }
        }

        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        public virtual void Insert()
        {
        }

        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// <value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        /// 返回
        public virtual bool CanEdit
        {
            get { return true; }
        }

        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        public virtual void Edit()
        {
        }

        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.</value>
        public virtual bool CanDelete
        {
            get { return true; }
        }

        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        public virtual void Delete()
        {
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以展开
        /// </summary>
        /// <value>返回<c>true</c>如果可以展开的话; 否则为, <c>false</c>.</value>
        public virtual bool CanExpand
        {
            get { return true; }
        }

        /// <summary>
        /// 展开数据列表视图
        /// </summary>
        public virtual void Expand()
        {
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以折叠
        /// </summary>
        /// <value>返回<c>true</c>如果可以折叠的话; 否则为, <c>false</c>.</value>
        public virtual bool CanCollaspe
        {
            get { return true; }
        }

        /// <summary>
        /// 折叠数据列表视图
        /// </summary>
        public virtual void Collaspe()
        {
        }

        /// <summary>
        /// 获取一个值决定当前数据列表是否可以刷新
        /// </summary>
        /// <value>返回<c>true</c>如果可以刷新的话; 否则为, <c>false</c>.</value>
        public virtual bool CanRefreshDataSource
        {
            get { return true; }
        }

        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        public virtual void RefreshDataSource()
        {
        }

        /// <summary>
        /// 数据插入前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataInserting;
        protected void OnDataInserting(object sender, CancelDataHandlerEventArgs e)
        {
            if (DataInserting != null)
                DataInserting(sender, e);
        }

        /// <summary>
        /// 数据插入后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataInserted;
        protected void OnDataInserted(object sender, DataHandlerEventArgs e)
        {
            if (DataInserted != null)
                DataInserted(sender, e);
        }

        /// <summary>
        /// 数据编辑前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataEditing;
        protected void OnDataEditing(object sender, CancelDataHandlerEventArgs e)
        {
            if (DataEditing != null)
                DataEditing(sender, e);
        }

        /// <summary>
        /// 数据编辑后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataEdited;
        protected void OnDataEdited(object sender, DataHandlerEventArgs e)
        {
            if (DataEdited != null)
                DataEdited(sender, e);
        }

        /// <summary>
        /// 数据删除前的事件
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataDeleting;
        protected void OnDataDeleting(object sender, CancelDataHandlerEventArgs e)
        {
            if (DataDeleting != null)
                DataDeleting(sender, e);
        }

        /// <summary>
        /// 数据删除后的事件
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataDeleted;
        protected void OnDataDeleted(object sender, DataHandlerEventArgs e)
        {
            if (DataDeleted != null)
                DataDeleted(sender, e);
        }

        /// <summary>
        /// 列表刷新后的事件
        /// </summary>
        public event EventHandler<EventArgs> DataRefreshed;
        protected void OnDataRefreshed(object sender, EventArgs e)
        {
            if (DataRefreshed != null)
                DataRefreshed(sender, e);
        }

        #endregion
    }
}
