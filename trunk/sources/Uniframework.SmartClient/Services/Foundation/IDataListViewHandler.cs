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
        /// <summary>
        /// Gets a value indicating whether this instance can set detail view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can set detail view; otherwise, <c>false</c>.
        /// </value>
        bool CanSetDetailView { get; }
        /// <summary>
        /// Sets the detail view.
        /// </summary>
        void SetDetailView();
        /// <summary>
        /// Gets a value indicating whether this instance can set layout view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can set layout view; otherwise, <c>false</c>.
        /// </value>
        bool CanSetLayoutView { get; }
        /// <summary>
        /// Sets the layout view.
        /// </summary>
        void SetLayoutView();
        /// <summary>
        /// Gets a value indicating whether this instance can select layout view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can select layout view; otherwise, <c>false</c>.
        /// </value>
        bool CanSelectLayoutView { get; }
        /// <summary>
        /// Selects the layout view.
        /// </summary>
        void SelectLayoutView();
        /// <summary>
        /// Gets a value indicating whether this instance can setting.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can setting; otherwise, <c>false</c>.
        /// </value>
        bool CanSetting { get; }
        /// <summary>
        /// Settings this instance.
        /// </summary>
        void Setting();
        /// <summary>
        /// Gets a value indicating whether this instance can show group panel.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can show group panel; otherwise, <c>false</c>.
        /// </value>
        bool CanShowGroupPanel { get; }
        /// <summary>
        /// Shows the group panel.
        /// </summary>
        void ShowGroupPanel();
        /// <summary>
        /// Gets a value indicating whether this instance can show footer.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can show footer; otherwise, <c>false</c>.
        /// </value>
        bool CanShowFooter { get; }
        /// <summary>
        /// Shows the footer.
        /// </summary>
        void ShowFooter();
    }
}
