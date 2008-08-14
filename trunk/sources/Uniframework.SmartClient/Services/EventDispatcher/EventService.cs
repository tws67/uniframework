using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.Utility;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 客户端异步事件分配服务
    /// </summary>
    public class EventService : IEventService, IDisposable
    {
        private static readonly int dispatcherYield = 5000;
        private Dictionary<string, EventDispatcher> eventDispatchers;
        private Dictionary<string, ISubscripterRegister> eventSubscripters;
        private object SyncObj = new object();

        public EventService()
        {
            eventDispatchers = new Dictionary<string, EventDispatcher>();
            eventSubscripters = new Dictionary<string, ISubscripterRegister>();
        }

        #region IEventDispatcherService Members

        public EventDispatcher GetEventDispatcher(string eventName)
        {
            if (eventName == null || eventName == string.Empty || !eventDispatchers.ContainsKey(eventName))
                return null;

            return eventDispatchers[eventName];            
        }

        public void RegisterEventDispatcher(string eventName, EventDispatcher eventDispatcher)
        {
            Guard.ArgumentNotNull(eventName, "eventName");
            Guard.ArgumentNotNull(eventDispatcher, "eventDispatcher");

            lock (SyncObj)
            {
                if (!eventDispatchers.ContainsKey(eventName))
                {
                    eventDispatchers.Add(eventName, eventDispatcher);
                    eventDispatchers[eventName].Start();
                }
            }
        }

        public void UnRegisterEventDispatcher(string eventName)
        {
            Guard.ArgumentNotNull(eventName, "eventName");

            lock (SyncObj)
            {
                if (eventDispatchers.ContainsKey(eventName))
                {
                    eventDispatchers[eventName].Subject.Disable();
                    eventDispatchers[eventName].Stop();
                    System.Threading.Thread.Sleep(dispatcherYield);
                    eventDispatchers.Remove(eventName);
                }
            }
        }

        public void ClearEventDispatcher()
        {
            foreach (string eventName in eventDispatchers.Keys)
            {
                UnRegisterEventDispatcher(eventName);
            }
        }

        public void StartEventDispatcher(string eventName)
        {
            Guard.ArgumentNotNull(eventName, "eventName");

            lock (SyncObj)
            {
                if (eventDispatchers.ContainsKey(eventName))
                {
                    eventDispatchers[eventName].Start();
                }
            }
        }

        public void StopEventDispatcher(string eventName)
        {
            Guard.ArgumentNotNull(eventName, "eventName");

            lock (SyncObj)
            {
                if (eventDispatchers.ContainsKey(eventName))
                {
                    eventDispatchers[eventName].Stop();
                }
            }
        }

        public void RegisterSubscripter(ISubscripterRegister subscripter)
        {
            Guard.ArgumentNotNull(subscripter, "subscripter");

            lock (SyncObj)
            {
                if (!eventSubscripters.ContainsKey(subscripter.Name))
                { 
                    subscripter.Register();
                    eventSubscripters.Add(subscripter.Name, subscripter);
                }
            }
        }

        public void UnRegisterSubscripter(string subscripterName)
        {
            Guard.ArgumentNotNull(subscripterName, "subscripterName");

            lock (SyncObj)
            {
                if (eventSubscripters.ContainsKey(subscripterName))
                {
                    eventSubscripters[subscripterName].UnRegister();
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    foreach (EventDispatcher dispatcher in eventDispatchers.Values)
                    {
                        dispatcher.Subject.Disable();
                        dispatcher.Stop();
                        System.Threading.Thread.Sleep(dispatcherYield);
                    }
                    disposed = true;
                }
            }
        }

        public void Close()
        {
            Dispose();
        }

        ~EventService()
        {
            Dispose(false);
        }
    }
}
