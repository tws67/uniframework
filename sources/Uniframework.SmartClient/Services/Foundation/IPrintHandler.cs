using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 打印接口
    /// </summary>
    public interface IPrintHandler
    {
        /// <summary>
        /// Gets a value indicating whether this instance can print.
        /// </summary>
        /// <value><c>true</c> if this instance can print; otherwise, <c>false</c>.</value>
        bool CanPrint { get; }
        /// <summary>
        /// Prints this instance.
        /// </summary>
        void Print();
        /// <summary>
        /// Gets a value indicating whether this instance can quick print.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can quick print; otherwise, <c>false</c>.
        /// </value>
        bool CanQuickPrint { get; }
        /// <summary>
        /// Quicks the print.
        /// </summary>
        void QuickPrint();
        /// <summary>
        /// Gets a value indicating whether this instance can preview.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can preview; otherwise, <c>false</c>.
        /// </value>
        bool CanPreview { get; }
        /// <summary>
        /// Previews this instance.
        /// </summary>
        void Preview();
        /// <summary>
        /// Gets a value indicating whether this instance can page setup.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can page setup; otherwise, <c>false</c>.
        /// </value>
        bool CanPageSetup { get; }
        /// <summary>
        /// Pages the setup.
        /// </summary>
        void PageSetup();
        /// <summary>
        /// Gets a value indicating whether this instance can design.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can design; otherwise, <c>false</c>.
        /// </value>
        bool CanDesign { get; }
        /// <summary>
        /// Designs this instance.
        /// </summary>
        void Design();
    }
}
