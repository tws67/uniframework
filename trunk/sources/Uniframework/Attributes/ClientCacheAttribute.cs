using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 客户端缓存属性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ClientCacheAttribute : Attribute
    {
        private string dataUpdateEvent;

        /// <summary>
        /// 无参构造函数，增加此构造函数主要是为了能够让XmlSerializer进行序列化
        /// </summary>
        public ClientCacheAttribute()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataUpdateEvent">缓存数据更新事件</param>
        public ClientCacheAttribute(string dataUpdateEvent)
        {
            this.dataUpdateEvent = dataUpdateEvent;
        }

        /// <summary>
        /// 获取数据更新事件名称
        /// </summary>
        public string DataUpdateEvent
        {
            get
            {
                return dataUpdateEvent;
            }
        }
    }
}
