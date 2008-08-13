using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    public class RemoteCaller : IRemoteCaller
    {
        public void RemoteCallFinished(InvokedResult result)
        {
            if (InvokedMethodFinished != null)
                InvokedMethodFinished(result);
        }

        public void RemoteCallError(InvokeErrorInfo error)
        {
            if (this.InvokeMethodFailed != null)
                this.InvokeMethodFailed(error);
        }

        #region IRemoteCaller Members

        public event InvokedFinishedHandler InvokedMethodFinished;

        public event InvokedErrorHandler InvokeMethodFailed;

        #endregion
    }
}
