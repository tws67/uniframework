using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 会话状态接口
    /// </summary>
    public interface ISessionState
    {
        /// <summary>
        /// 会话标识
        /// </summary>
        string SessionId { get; set; }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">标识</param>
        /// <returns></returns>
        object this[object key] { get; set; }
        /// <summary>
        /// 移除所有会话状态
        /// </summary>
        void RemoveAll();
        /// <summary>
        /// 移除指定标识的实例
        /// </summary>
        /// <param name="key">标识</param>
        void Remove(object key);
        /// <summary>
        /// 判断是否存在该key值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(object key);
    }
}
