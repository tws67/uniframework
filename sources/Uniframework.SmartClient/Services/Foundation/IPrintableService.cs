using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 打印服务接口
    /// </summary>
    public interface IPrintableService
    {
        /// <summary>
        /// Registers the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void Register(IPrintHandler handler);
        /// <summary>
        /// Registers the specified UI element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        void Register(object uiElement);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void UnRegister(IPrintHandler handler);
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        void UnRegister(object uiElement);
    }
}
