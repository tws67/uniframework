using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Microsoft.Practices.ObjectBuilder;

using Uniframework.Client;
using Uniframework.Services;

namespace Uniframework.StartUp.Strategies
{
    /// <summary>
    /// 事件连接器
    /// </summary>
    public class EventConnectStrategy : BuilderStrategy
    {
        public EventConnectStrategy()
        {
        }

        #region Assistant function 

        private void EventToServer(object component, bool isReigster)
        {
            //Program.Logger.Debug("检查组件[" + component.GetType().Name + "]的服务器订阅事件");
            MethodInfo[] methods = component.GetType().GetMethods();
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attrs = methodInfo.GetCustomAttributes(typeof(EventSubscriberAttribute), true);
                if (attrs.Length > 0)
                {
                    EventSubscriberAttribute subscriberAttr = (EventSubscriberAttribute)attrs[0];
                    //Program.Logger.Debug("发现一个[" + subscriberAttr.Topic + "]事件的订阅方法[" + methodInfo.Name + "]");
                    if (isReigster)
                    {
                        //Program.Logger.Debug("向服务器注册事件");
                        ClientEventDispatcher.Instance.RegisterEventSubscriber(subscriberAttr.Topic, component, methodInfo);
                    }
                    else
                    {
                        //Program.Logger.Debug("向服务器注销事件");
                        ClientEventDispatcher.Instance.UnRegisterEventSubscriber(subscriberAttr.Topic, methodInfo, component);
                    }
                }
            }
        }

        #endregion 

        #region IBuilderStrategy Members

        /// <summary>
        /// Forwards the <see cref="EventTopic"/> related attributes processing to the <see cref="EventInspector"/>
        /// for registering publishers and/or subscribers.
        /// </summary>
        public override object BuildUp(IBuilderContext context, Type t, object existing, string id)
        {
            EventToServer(existing, true);
            return base.BuildUp(context, t, existing, id);
        }

        /// <summary>
        /// Forwards the <see cref="EventTopic"/> related attributes processing to the <see cref="EventInspector"/>
        /// for unregistering publishers and/or subscribers.
        /// </summary>
        public override object TearDown(IBuilderContext context, object item)
        {
            EventToServer(item, false);
            return base.TearDown(context, item);
        }

        #endregion
    }
}
