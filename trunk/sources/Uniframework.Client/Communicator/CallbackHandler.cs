using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Uniframework.Services;

namespace Uniframework.Client
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class CallbackHandler : IInvokeCallback
    {
        private SynchronizationContext syncContext = null;

        /// <summary>
        /// Notifies the data change.
        /// </summary>
        /// <param name="eventResult">The event result.</param>
        public void NotifyDataChange(EventResultData eventResult)
        {
            EventResultData data = eventResult;
            if (data != null)
            {
                ClientEventDispatcher.Instance.DispachEvent(data.Topic, data.Args);
            }
        }

        public bool Ping()
        {
            return true;
        }
    }
}
