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
    /// Tcp“Ï≥£¿‡
    /// </summary>
    public class TcpException : NetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TcpException(string message)
            : base(message)
        {
        }

    }
}
