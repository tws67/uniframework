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
    /// ������ϢͨѶ�Ļػ���
    /// </summary>
    public class MessageBlockSession : TcpSession
    {
        private int maxMessageLength = 64 * 1024;

        /// <summary>
        /// ��Ϣ��󳤶�
        /// </summary>
        internal int MaxMessageLength
        {
            get { return maxMessageLength; }
            set { maxMessageLength = value; }
        }

        /// <summary>
        /// �޸�Base��Ķ��塣
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
        /// ���յ�readCount�����ݡ�
        /// </summary>
        /// <param name="readCount">���ݶ�ȡ�ĸ���</param>
        internal protected override void ReceivedData(int readCount)
        {
            //DataBlock dataBlock = this.Buffer;

            //System.Diagnostics.Trace.WriteLine("-MB Session receive data count:" + readCount);
            //System.Diagnostics.Trace.WriteLine("-Raw data:" + Convert.ToBase64String(dataBlock.ArraySegment.Array, dataBlock.ArraySegment.Offset, dataBlock.ArraySegment.Count+readCount));
            //System.Diagnostics.Trace.WriteLine("-Orgin Data:" + Encoding.Unicode.GetString(dataBlock.ArraySegment.Array, dataBlock.ArraySegment.Offset, dataBlock.ArraySegment.Count+readCount));

            if (!Message.Head.IsFull)
            {
                //��ȡ����ͷ
                Message.Head.WriteIndex += readCount;
                if (Message.Head.IsFull)
                {
                    //�����Ϣͷ�Ľ���
                    Message.CreateBodyByHead(MaxMessageLength);
                    if (Message.BodyLength == 0)
                    {
                        ReceivedMessage();
                        return;
                    }
                    //׼��������Ϣ�岿��
                    base.Buffer = Message.Body;
                }
            }
            else
            {
                //��ȡ���ݿ�
                Message.Body.WriteIndex += readCount;
                if (messageBlock.Body.IsFull)//���ݿ�����
                {
                    ReceivedMessage();
                }
                //���ݿ�δ������������
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

            //��ʼ������һ�ε�����
            messageBlock = new MessageBlock();
            base.Buffer = Message.Head;
        }
    }
}
