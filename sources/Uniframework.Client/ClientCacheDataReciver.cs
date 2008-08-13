using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Client
{
    public class ClientCacheDataReciver
    {
        private string dataUpdateEvent;
        private object data;
        private object param;
        private MethodInfo method;

        public ClientCacheDataReciver(string dataUpdateEvent, object data, MethodInfo method, object param)
        {
            if (dataUpdateEvent != null) ClientEventDispatcher.Instance.RegisterEventSubscriber(dataUpdateEvent, this, typeof(ClientCacheDataReciver).GetMethod("OnDataUpdated"));
            this.data = data;
            this.param = param;
            this.method = method;
        }

        public void OnDataUpdated(object sender, EventArgs e)
        {
            this.data = CommunicateProxy.InvokeCommand(method, param);
        }

        public object CachedData
        {
            get
            {
                return data;
            }
        }
    }
}
