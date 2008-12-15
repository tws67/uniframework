using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表处理器接口
    /// </summary>
    public interface IDataListViewHandler
    {
        /// <summary>
        /// Occurs when [data grid actived].
        /// </summary>
        event EventHandler Enter;
        /// <summary>
        /// Occurs when [data grid deactived].
        /// </summary>
        event EventHandler Leave;

        /// <summary>
        /// Inits the data.
        /// </summary>
        void Initilize();
        /// <summary>
        /// Gets a value indicating whether this instance can insert.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can insert; otherwise, <c>false</c>.
        /// </value>
        bool CanInsert { get; }
        /// <summary>
        /// Inserts this instance.
        /// </summary>
        void Insert();
        /// <summary>
        /// Gets a value indicating whether this instance can edit.
        /// </summary>
        /// <value><c>true</c> if this instance can edit; otherwise, <c>false</c>.</value>
        bool CanEdit { get; }
        /// <summary>
        /// Edits this instance.
        /// </summary>
        void Edit();
        /// <summary>
        /// Gets a value indicating whether this instance can delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </value>
        bool CanDelete { get; }
        /// <summary>
        /// Deletes this instance.
        /// </summary>
        void Delete();
        /// <summary>
        /// Gets a value indicating whether this instance can expand.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can expand; otherwise, <c>false</c>.
        /// </value>
        bool CanExpand { get; }
        /// <summary>
        /// Expands this instance.
        /// </summary>
        void Expand();
        /// <summary>
        /// Gets a value indicating whether this instance can collaspe.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can collaspe; otherwise, <c>false</c>.
        /// </value>
        bool CanCollaspe { get; }
        /// <summary>
        /// Collaspes this instance.
        /// </summary>
        void Collaspe();
        /// <summary>
        /// Gets a value indicating whether this instance can filter.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can filter; otherwise, <c>false</c>.
        /// </value>
        bool CanFilter { get; }
        /// <summary>
        /// Filters this instance.
        /// </summary>
        void Filter();
        /// <summary>
        /// Gets a value indicating whether this instance can refresh.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can refresh; otherwise, <c>false</c>.
        /// </value>
        bool CanRefresh { get; }
        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();
    }
}
