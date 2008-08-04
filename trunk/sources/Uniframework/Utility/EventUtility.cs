using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 事件触发工具类
    /// </summary>
    public static class EventUtility
    {
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件实例</param>
        /// <param name="sender">事件触发者</param>
        /// <param name="e">事件参数</param>
        public static void Raise<T>(EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T CreateEventArgs<T>() where T : EventArgs;

        /// <summary>
        /// 触发指定的事件
        /// </summary>
        /// <typeparam name="T">事件类型（泛型）</typeparam>
        /// <param name="handler">事件句柄</param>
        /// <param name="sender">事件触发者</param>
        /// <param name="createEventArgs">事件参数</param>
        public static void Raise<T>(EventHandler<T> handler, object sender, CreateEventArgs<T> createEventArgs) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, createEventArgs());
            }
        }
    }
}
