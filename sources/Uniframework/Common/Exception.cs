// ***************************************************************
//  version:  1.0   date: 11/27/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 逻辑上的异常
    /// </summary>
    public class LogicException : ApplicationException
    {
        public LogicException(string message)
            :base(message)
            {

            }
    }

    /// <summary>
    /// 运行发生的异常
    /// </summary>
    public class RuntimeException : ApplicationException
    {
        public RuntimeException(string message)
            : base(message)
        {

        }
    }
}
