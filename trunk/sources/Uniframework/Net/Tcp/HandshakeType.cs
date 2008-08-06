// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Net
{
    /// <summary>
    /// 握手枚举
    /// </summary>
    public enum HandshakeType
    {
        /// <summary>
        /// 客户端Hello 
        /// </summary>
        ClientHello,   
        /// <summary>
        /// 服务器Hello
        /// </summary>
        ServerHello,  

        /// <summary>
        /// 交换服务器公匙
        /// </summary>
        ServerKeyExchange, 
        /// <summary>
        /// 交换客户端共匙
        /// </summary>
        ClientKeyExchange, 

        /// <summary>
        /// 对称加密方法私匙
        /// </summary>
        ClientSymmetricKey,
        /// <summary>
        /// 服务器收到客户端对称加密方法私匙的回复
        /// </summary>
        ServerGetSymmetricKey, 

        /// <summary>
        /// 对称加密方法初始化向量
        /// </summary>
        ClientSymmetricIV, 
        /// <summary>
        /// 服务器收到客户端对称加密方法初始化向量的回复
        /// </summary>
        ServerGetSymmetricIV,

        /// <summary>
        /// 服务器认证结束
        /// </summary>
        ServerFinished, 　
        /// <summary>
        /// 客户端认证结束
        /// </summary>
        ClientFinished,　　

        /// <summary>
        /// 握手成功
        /// </summary>
        OK
    }
}
