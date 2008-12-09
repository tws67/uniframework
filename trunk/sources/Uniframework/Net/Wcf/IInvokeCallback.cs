using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Uniframework.Services;

namespace Uniframework
{
    /// <summary>
    /// Wcf服务器端回调接口
    /// </summary>
    public interface IInvokeCallback
    {
        /// <summary>
        /// 通知事件结果变化事件
        /// </summary>
        /// <param name="data">The data.</param>
        [OperationContract(IsOneWay = true)]
        void NotifyDataChange(EventResultData data);
        /// <summary>
        /// 客户端Ping操作
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool Ping();
    }
}
