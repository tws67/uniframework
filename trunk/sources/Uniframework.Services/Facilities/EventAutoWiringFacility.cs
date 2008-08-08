using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Uniframework.Services.Facilities
{
    /// <summary>
    /// 事件发布者、订阅者连接器
    /// </summary>
    public class EventAutoWiringFacility : AbstractFacility
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EventAutoWiringFacility()
        {
        }

        protected override void Init()
        {
            Kernel.AddComponent("event.dispatcher", typeof(IEventDispatcher), typeof(EventDispatcher));
            Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
        }

        #region Assistant function

        private void OnComponentModelCreated(ComponentModel model)
        {
            RegisterEventPublisher(model);
            RegisterEventSubstriber(model);
        }

        /// <summary>
        /// Registers the event publisher.
        /// </summary>
        /// <param name="model">The model.</param>
        private void RegisterEventPublisher(ComponentModel model)
        {
            EventInfo[] events = model.Service.GetEvents();

            foreach (EventInfo e in events)
            {
                object[] attrs = e.GetCustomAttributes(typeof(EventPublisherAttribute), true);
                if (attrs.Length > 0)
                {
                    EventPublisherAttribute publisherAttr = (EventPublisherAttribute)attrs[0];
                    IEventDispatcher dispatcher = Kernel[typeof(IEventDispatcher)] as IEventDispatcher;
                    if (dispatcher != null)
                    {
                        dispatcher.RegisterEventPublisher(
                            publisherAttr.Topic,
                            publisherAttr.Description,
                            publisherAttr.EventScope,
                            e);
                    }
                }
            }
        }

        /// <summary>
        /// Registers the event substriber.
        /// </summary>
        /// <param name="model">The model.</param>
        private void RegisterEventSubstriber(ComponentModel model)
        {
            MethodInfo[] methods = model.Implementation.GetMethods();
            foreach (MethodInfo methodInfo in methods)
            {
                object[] attrs = methodInfo.GetCustomAttributes(typeof(EventSubscriberAttribute), true);
                if (attrs.Length > 0)
                {
                    EventSubscriberAttribute subscriberAttr = (EventSubscriberAttribute)attrs[0];
                    IEventDispatcher dispatcher = Kernel[typeof(IEventDispatcher)] as IEventDispatcher;
                    dispatcher.RegisterEventSubscriber(subscriberAttr.Topic, methodInfo, model.Service);
                }
            }
        }

        #endregion
    }
}
