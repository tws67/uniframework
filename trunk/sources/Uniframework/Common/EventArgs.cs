using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 公共事件参数类，此类提供了一个泛型的事件参数
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        T data;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data"></param>
        public EventArgs(T data)
        {
            this.data = data;
        }

        /// <summary>
        /// 参数包含的数据资料
        /// </summary>
        public T Data
        {
            get
            {
                return data;
            }
        }
    }
}
