using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 事件监听器，每个Listener负责收集一个Topic事件源的所有参数信息
    /// </summary>
    internal class EventListener : IDisposable
    {
        Queue<EventArgs> argsQueue = new Queue<EventArgs>();
        EventInfo eventInfo;
        object publisher;
        Delegate handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="publisher">The publisher.</param>
        public EventListener(EventInfo eventInfo, object publisher)
        {
            this.eventInfo = eventInfo;
            this.publisher = publisher;
            BindingEvent();
        }

        #region Assistant function

        /// <summary>
        /// Bindings the event.
        /// </summary>
        private void BindingEvent()
        {
            try
            {
                handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, typeof(EventListener).GetMethod("CommonEventHandle"));
                eventInfo.AddEventHandler(publisher, handler);
            }
            catch (Exception ex)
            {
                throw new Exception("在绑定接口 [" +
                            eventInfo.DeclaringType.ToString() +
                            "] 上定义的事件 [" + eventInfo.Name + "] 到公共事件处理方法时出错。请确定事件的委托类型为 [EventHandler]", ex);
            }
        }

        /// <summary>
        /// Commons the event handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void CommonEventHandle(object sender, EventArgs e)
        {
            argsQueue.Enqueue(e);
        }

        /// <summary>
        /// Dequeues all event args.
        /// </summary>
        /// <returns></returns>
        public EventArgs[] DequeueAllEventArgs()
        {
            List<EventArgs> list = new List<EventArgs>();
            while (argsQueue.Count != 0)
            {
                list.Add(argsQueue.Dequeue());
            }
            return list.ToArray();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            eventInfo.RemoveEventHandler(publisher, handler);
        }

        #endregion
    }
}
