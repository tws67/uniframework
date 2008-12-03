using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Uniframework.Services;

namespace Uniframework
{
    /// <summary>
    /// Wcf回调接口
    /// </summary>
    public interface IWcfCallback
    {
        [OperationContract(IsOneWay = true)]
        void CallBackInvoke(ChatData data);
        [OperationContract(IsOneWay = true)]
        void NotifyDataChange(EventResultData data);
        [OperationContract]
        bool Ping();
    }
}
