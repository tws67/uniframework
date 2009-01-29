using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表处理器接口
    /// </summary>
    public interface IDataListHandler
    {
        #region Methods && Properties

        /// <summary>
        /// Occurs when [data grid actived].
        /// </summary>
        event EventHandler Enter;
        /// <summary>
        /// Occurs when [data grid deactived].
        /// </summary>
        event EventHandler Leave;

        /// <summary>
        /// 初始化数据列表操作只在数据列表第一次加载时使用
        /// </summary>
        void Initilize();
        /// <summary>
        /// 获取一个值决定当前可否插入新的数据资料
        /// </summary>
        /// <value>
        /// 	返回<c>true</c>如果可以插入的话; 否则为, <c>false</c>.
        /// </value>
        bool CanInsert { get; }
        /// <summary>
        /// 插入新的数据资料
        /// </summary>
        void Insert();
        /// <summary>
        /// 获取一个值决定当前可否编辑选定数据资料
        /// </summary>
        /// 返回<value><c>true</c>如果可以编辑的话; 否则为, <c>false</c>.</value>
        bool CanEdit { get; }
        /// <summary>
        /// 编辑选定数据资料
        /// </summary>
        void Edit();
        /// <summary>
        /// 获取一个值决定当前可否删除选定数据资料
        /// </summary>
        /// <value>
        /// 	返回<c>true</c>如果可以删除的话; 否则为, <c>false</c>.
        /// </value>
        bool CanDelete { get; }
        /// <summary>
        /// 删除选定数据资料
        /// </summary>
        void Delete();
        /// <summary>
        /// 获取一个值决定当前数据列表是否可以展开
        /// </summary>
        /// <value>
        /// 	返回<c>true</c>如果可以展开的话; 否则为, <c>false</c>.
        /// </value>
        bool CanExpand { get; }
        /// <summary>
        /// 展开数据列表视图
        /// </summary>
        void Expand();
        /// <summary>
        /// 获取一个值决定当前数据列表是否可以折叠
        /// </summary>
        /// <value>
        /// 	返回<c>true</c>如果可以折叠的话; 否则为, <c>false</c>.
        /// </value>
        bool CanCollaspe { get; }
        /// <summary>
        /// 折叠数据列表视图
        /// </summary>
        void Collaspe();
        /// <summary>
        /// 获取一个值决定当前数据列表是否可以刷新
        /// </summary>
        /// <value>
        /// 	返回<c>true</c>如果可以刷新的话; 否则为, <c>false</c>.
        /// </value>
        bool CanRefreshDataSource { get; }
        /// <summary>
        /// 刷新数据列表视图
        /// </summary>
        void RefreshDataSource();

        #endregion

        #region Events

        /// <summary>
        /// 数据插入前的事件
        /// </summary>
        event EventHandler<CancelDataHandlerEventArgs> DataInserting;
        /// <summary>
        /// 数据插入后的事件
        /// </summary>
        event EventHandler<DataHandlerEventArgs> DataInserted;

        /// <summary>
        /// 数据编辑前的事件
        /// </summary>
        event EventHandler<CancelDataHandlerEventArgs> DataEditing;
        /// <summary>
        /// 数据编辑后的事件
        /// </summary>
        event EventHandler<DataHandlerEventArgs> DataEdited;

        /// <summary>
        /// 数据删除前的事件
        /// </summary>
        event EventHandler<CancelDataHandlerEventArgs> DataDeleting;
        /// <summary>
        /// 数据删除后的事件
        /// </summary>
        event EventHandler<DataHandlerEventArgs> DataDeleted;

        /// <summary>
        /// 列表刷新后的事件
        /// </summary>
        event EventHandler<EventArgs> DataRefreshed;

        #endregion
    }

    #region Data Handler Event Args

    /// <summary>
    /// 数据列表处理参数
    /// </summary>
    public class DataHandlerEventArgs : EventArgs
    {
        private string user;
        private List<object> list = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataHandlerEventArgs"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="list">The list.</param>
        public DataHandlerEventArgs(string user, IList<object> list)
        {
            this.user = user;
            this.list.AddRange(list);
        }

        public string User
        {
            get { return user; }
        }

        public IList<object> List
        {
            get { return list; }
        }
    }

    /// <summary>
    /// 数据列表处理取消参数
    /// </summary>
    public class CancelDataHandlerEventArgs : DataHandlerEventArgs
    {
        private bool cancel = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelDataHandlerEventArgs"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="list">The list.</param>
        public CancelDataHandlerEventArgs(string user, IList<object> list)
            : base(user, list)
        { }

        /// <summary>
        /// 操作是否取消
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }

    #endregion
}
