using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 通用信息标识接口，用于对板卡适配器、驱动程序等的标识
    /// </summary>
    public interface IVersionInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name {get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; set; }
        /// <summary>
        /// 扩展属性信息
        /// </summary>
        Dictionary<string, object> Properties { get;}
    }
}
