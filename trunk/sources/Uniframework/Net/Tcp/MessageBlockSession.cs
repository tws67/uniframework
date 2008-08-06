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
    /// 基于消息通讯的回话类
    /// </summary>
    public class MessageBlockSession : TcpSession
    {
        private int maxMessageLength = 64 * 1024;

        /// <summary>
        /// 消息最大长度
        /// </summary>
        internal int MaxMessageLength
        {
            get { return maxMessageLength; }
            set { maxMessageLength = value; }
        }

        /// <summary>
        /// 修改Base类的定义。
        /// </summary>
        protected override void OnCreate()
        {
            messageBlock = new MessageBlock();
            base.Buffer = messageBlock.Head;
        }

        private MessageBlock messageBlock;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public MessageBlock Message
        {
            get { return messageBlock; }
        }

        /// <summary>
        /// Occurs when [on received message block].
        /// </summary>
        public event EventHandler<MessageBlockArgs> OnReceivedMessageBlock;

        /// <summary>
        /// 接收到readCount个数据。
        /// </summary>
        /// <param name="readCount">数据读取的个数</param>
        internal protected override void ReceivedData(int readCount)
        {
            //DataBlock dataBlock = this.Buffer;

            //System.Diagnostics.Trace.WriteLine("-MB Session receive data count:" + readCount);
            //System.Diagnostics.Trace.WriteLine("-Raw data:" + Convert.ToBase64String(dataBlock.ArraySegment.Array, dataBlock.ArraySegment.Offset, dataBlock.ArraySegment.Count+readCount));
            //System.Diagnostics.Trace.WriteLine("-Orgin Data:" + Encoding.Unicode.GetString(dataBlock.ArraySegment.Array, dataBlock.ArraySegment.Offset, dataBlock.ArraySegment.Count+readCount));

            if (!Message.Head.IsFull)
            {
                //读取数据头
                Message.Head.WriteIndex += readCount;
                if (Message.Head.IsFull)
                {
                    //完成消息头的接收
                    Message.CreateBodyByHead(MaxMessageLength);
                    if (Message.BodyLength == 0)
                    {
                        ReceivedMessage();
                        return;
                    }
                    //准备接收消息体部分
                    base.Buffer = Message.Body;
                }
            }
            else
            {
                //读取数据块
                Message.Body.WriteIndex += readCount;
                if (messageBlock.Body.IsFull)//数据块已满
                {
                    ReceivedMessage();
                }
                //数据块未满，继续接收
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("MB-S::{0:0000}", ID);
        }

        /// <summary>
        /// Receiveds the message.
        /// </summary>
        private void ReceivedMessage()
        {
            EventHandler<MessageBlockArgs> temp = OnReceivedMessageBlock;
            if (temp != null)
            {
                temp(this, new MessageBlockArgs(Message));
            }

            //开始接收下一次的数据
            messageBlock = new MessageBlock();
            base.Buffer = Message.Head;
        }
    }
}
