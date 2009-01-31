using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据列表服务接口
    /// </summary>
    public interface IDataListViewService
    {
        /// <summary>
        /// Registers the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void Register(IDataListView handler);
        /// <summary>
        /// Registers the specified datagrid.
        /// </summary>
        /// <param name="datagrid">The datagrid.</param>
        void Register(object uiElement);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void UnRegister(IDataListView handler);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="datagrid">The datagrid.</param>
        void UnRegister(object uiElement);
    }
}
