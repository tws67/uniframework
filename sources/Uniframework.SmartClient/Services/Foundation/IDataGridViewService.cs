using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 数据表格服务接口
    /// </summary>
    public interface IDataGridViewService
    {
        /// <summary>
        /// Registers the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void Register(IDataGridViewHandler handler);
        /// <summary>
        /// Registers the specified datagrid.
        /// </summary>
        /// <param name="datagrid">The datagrid.</param>
        void Register(object datagrid);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void UnRegister(IDataGridViewHandler handler);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="datagrid">The datagrid.</param>
        void UnRegister(object datagrid);
    }
}
