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
    /// 数据块事件参数类
    /// </summary>
    public class DataBlockArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataBlockArgs"/> class.
        /// </summary>
        /// <param name="dataBlock">The data block.</param>
        public DataBlockArgs(DataBlock dataBlock)
        {
            this.dataBlock = dataBlock;
        }

        private DataBlock dataBlock;

        public DataBlock DataBlock
        {
            get { return dataBlock; }
        }

    };

    /// <summary>
    /// 消息块事件参数类
    /// </summary>
    public class MessageBlockArgs : System.EventArgs
    {
        MessageBlock messageBlock;

        /// <summary>
        /// Gets the message block.
        /// </summary>
        /// <value>The message block.</value>
        public MessageBlock MessageBlock
        {
            get { return messageBlock; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlockArgs"/> class.
        /// </summary>
        /// <param name="messageBlock">The message block.</param>
        public MessageBlockArgs(MessageBlock messageBlock)
        {
            this.messageBlock = messageBlock;
        }

    }

}
