using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// Wcf通道接口
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IWcfCallback))]
    public interface IWcfChannel
    {
        /// <summary>
        /// Invokes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [OperationContract]
        byte[] Invoke(byte[] data);
    }
}
