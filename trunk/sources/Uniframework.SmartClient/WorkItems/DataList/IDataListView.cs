using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表视图
    /// </summary>
    public interface IDataListView : IDataView
    {
        /// <summary>
        /// 数据列表处理器
        /// </summary>
        /// <value>The presenter.</value>
        IDataListHandler DataListHandler { get; }
        /// <summary>
        /// 数据列表视图只读属性
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        bool ReadOnly { get; }
        /// <summary>
        /// 当视图获得输入焦点时触发此事件
        /// </summary>
        event EventHandler Enter;
        /// <summary>
        /// 当视力失去输入焦点时触发此事件
        /// </summary>
        event EventHandler Leave;
    }
}
