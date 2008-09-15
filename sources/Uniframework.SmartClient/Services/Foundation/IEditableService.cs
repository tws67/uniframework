using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 编辑器管理器
    /// </summary>
    public interface IEditableService
    {
        /// <summary>
        /// Register an EditHandler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void Register(IEditHandler handler);

        /// <summary>
        /// Register an UI element which is wrapped by an adapter to support the IEditHandler interface.
        /// </summary>
        void Register(object uiElement);

        /// <summary>
        /// Deregister an EditHandler. The method does not throw an exception if it is called more than
        /// once for the same object.
        /// </summary>
        /// <param name="handler">The handler.</param>
        void UnRegister(IEditHandler handler);

        /// <summary>
        /// Deregister an UI element. The method does not throw an exception if it is called more than
        /// once for the same object.
        /// </summary>
        void UnRegister(object uiElement);
    }
}
