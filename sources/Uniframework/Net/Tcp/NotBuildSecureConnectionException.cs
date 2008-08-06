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
    ///  ��ȫ����δ�������쳣
    /// </summary>
    public class NotBuildSecureConnectionException : NetSecureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotBuildSecureConnectionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NotBuildSecureConnectionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotBuildSecureConnectionException"/> class.
        /// </summary>
        public NotBuildSecureConnectionException()
            : base("������δ������ȫͨ�����ӵ�Զ��ͨ�š�")
        {
        }
    }
}
