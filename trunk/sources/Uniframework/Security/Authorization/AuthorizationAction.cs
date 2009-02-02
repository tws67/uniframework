using System;
using System.Collections;
using System.Collections.Generic;

namespace Uniframework.Security
{ 
    /// <summary>
    /// 对资源的授权操作
    /// </summary>
    public enum AuthorizationAction
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Deny,
        /// <summary>
        /// 可用
        /// </summary>
        Allow
    }
}