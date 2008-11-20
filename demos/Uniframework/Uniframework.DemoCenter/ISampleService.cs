using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.DemoCenter
{
    /// <summary>
    /// 框架基本功能演示
    /// </summary>
    [RemoteService]
    public interface ISampleService
    {
        /// <summary>
        /// 普通方法 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [RemoteMethod]
        string Hello(string username);

        /// <summary>
        /// 离线方法
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [RemoteMethod(true)]
        string HelloOffline(string username);

        /// <summary>
        /// 缓存方法
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [ClientCache(Constants.Event_TimeChanged)]
        [RemoteMethod(true)]
        string Hello4Cache(string username);

        /// <summary>
        /// 简单事件
        /// </summary>
        [EventPublisher(Constants.Event_SamplePublisher)]
        event EventHandler<EventArgs<string>> SampleEvent;

        /// <summary>
        /// 缓存方法所依赖的事件
        /// </summary>
        [EventPublisher(Constants.Event_TimeChanged)]
        event EventHandler TimeChanged;
    }
}
