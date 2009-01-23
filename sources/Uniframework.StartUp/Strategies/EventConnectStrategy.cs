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
    /// �¼�������
    /// </summary>
    public class EventConnectStrategy : BuilderStrategy
    {
        public EventConnectStrategy()
        {
        }

        #region Assistant function 

        private void EventToServer(object component, bool isReigster)
        {
            //Program.Logger.Debug("������[" + component.GetType().Name + "]�ķ����������¼�");
            MethodInfo[] methods = component.GetType().GetMethods();
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attrs = methodInfo.GetCustomAttributes(typeof(EventSubscriberAttribute), true);
                if (attrs.Length > 0)
                {
                    EventSubscriberAttribute subscriberAttr = (EventSubscriberAttribute)attrs[0];
                    //Program.Logger.Debug("����һ��[" + subscriberAttr.Topic + "]�¼��Ķ��ķ���[" + methodInfo.Name + "]");
                    if (isReigster)
                    {
                        //Program.Logger.Debug("�������ע���¼�");
                        ClientEventDispatcher.Instance.RegisterEventSubscriber(subscriberAttr.Topic, component, methodInfo);
                    }
                    else
                    {
                        //Program.Logger.Debug("�������ע���¼�");
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
