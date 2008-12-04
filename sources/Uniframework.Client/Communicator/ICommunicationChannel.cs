using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client
{
    /// <summary>
    /// 通信通道定义客户端到服务器端进行通信的通道
    /// </summary>
    public interface ICommunicationChannel
    {
        /// <summary>
        /// 调用服务器端的方法
        /// </summary>
        /// <param name="data">方法调用的字节流</param>
        /// <returns>服务器返回的字节流</returns>
        byte[] Invoke(byte[] data);
    }
}
