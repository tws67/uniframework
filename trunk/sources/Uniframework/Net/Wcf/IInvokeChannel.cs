using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// Wcf远程调用通道
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IInvokeCallback))]
    public interface IInvokeChannel
    {
        /// <summary>
        /// 调用远程方法
        /// </summary>
        /// <param name="data">调用数据包</param>
        /// <returns>以字节流返回的调用结果</returns>
        [OperationContract]
        byte[] Invoke(byte[] data);
    }
}
