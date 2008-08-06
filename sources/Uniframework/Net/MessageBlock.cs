// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Net
{
    /// <summary>
    /// 消息块类
    /// </summary>
    public class MessageBlock
    {
        #region Fields
        DataBlock body;
        DataBlock head;
        MessageBlockType type;
        public const int HeadLength = 16;
        int bodyLength;
        int parameter;
        #endregion

        #region Constractor

        /// <summary>
        /// 创建一个空MessageBlock,只包含头信息
        /// </summary>
        public MessageBlock()
        {
            Head = new DataBlock(HeadLength);
        }

        /// <summary>
        /// 构造一个包含应用数据的消息
        /// </summary>
        /// <param name="data">应用数据</param>
        public MessageBlock(Byte[] data)
            : this(data, 0, data.Length)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlock"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        public MessageBlock(Byte[] data, int startIndex, int length)
            : this(new DataBlock(data, startIndex, length))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlock"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public MessageBlock(DataBlock body)
            : this(MessageBlockType.AppData, 0, body)
        {
        }

        /// <summary>
        /// 创建一个特定类型的MessageBlok
        /// </summary>
        /// <param name="type"></param>
        public MessageBlock(MessageBlockType type)
            : this(type, 0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlock"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameter">The parameter.</param>
        public MessageBlock(MessageBlockType type, int parameter)
            : this(type, parameter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlock"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="body">The body.</param>
        public MessageBlock(MessageBlockType type, int parameter, DataBlock body)
            : this()
        {
            Type = type;
            Parameter = parameter;

            if (body != null)
                Body = body;

            CreateHead();
        }
        #endregion

        #region Property

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageBlockType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// 消息参数
        /// </summary>
        public int Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        /// <summary>
        /// 消息数据部分
        /// </summary>
        public DataBlock Body
        {
            get { return body; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                body = value; bodyLength = value.DataLength;
            }
        }

        /// <summary>
        /// 消息头部分
        /// </summary>
        public DataBlock Head
        {
            get { return head; }
            set { head = value; }
        }

        /// <summary>
        ///  消息体大小
        /// </summary>
        public int BodyLength
        {
            get
            {
                return bodyLength;
            }
        }

        /// <summary>
        /// 消息总的大小
        /// </summary>
        public int Length
        {
            get
            {
                return HeadLength + BodyLength;
            }
        }

        #endregion

        #region Public Methord

        /// <summary>
        /// Toes the data block.
        /// </summary>
        /// <returns></returns>
        public DataBlock ToDataBlock()
        {
            return Head + Body;
        }

        /// <summary>
        /// 根据消息设置，生成报文头信息
        /// </summary>
        public void CreateHead()
        {
            if (Head == null || Head.Buffer == null)
            {
                Head = new DataBlock(HeadLength);
            }

            byte[] result = this.head.Buffer;

            byte[] lengthBytes = MyConvert.ToBytes(Length);
            byte[] typeBytes = MyConvert.ToBytes((int)Type);
            byte[] parameterBytes = MyConvert.ToBytes(Parameter);

            int index = 0;
            //添加长度信息
            Array.Copy(lengthBytes, 0, result, index, 4);
            index += 4;
            //添加类型信息
            Array.Copy(typeBytes, 0, result, index, 4);
            index += 4;
            //添加参数信息
            Array.Copy(parameterBytes, 0, result, index, 4);

            //扩展信息，未使用，置为0
            index += 8;

            this.Head.WriteIndex = HeadLength;
        }

        /// <summary>
        /// 从数据得到一个消息
        /// </summary>
        /// <param name="data">包含消息的数据</param>
        /// <param name="getHeadData">是否得到头信息</param>
        public void FromBytes(byte[] data, bool getHeadData, int maxBodyLength)
        {
            int index = 0;
            int length = (int)MyConvert.ToUInt32(data, index);
            bodyLength = length - HeadLength;
            if (bodyLength > maxBodyLength)
            {
                throw new NetException(string.Format("message is too large {0}/{1}", bodyLength, maxBodyLength));
            }

            index += 4;
            Type = (MessageBlockType)MyConvert.ToUInt32(data, index);
            index += 4;
            Parameter = (int)MyConvert.ToUInt32(data, index);
            if (!getHeadData && length > HeadLength)
            {
                index += 4;
                body = new DataBlock(length - HeadLength);
                Array.Copy(data, index, body.Buffer, body.WriteIndex, body.WritableLength);
                body.WriteIndex += body.WritableLength;
            }
            else if (BodyLength >= 0)
            {
                body = new DataBlock(length - HeadLength);
            }
            else
            {
                throw new NetException("Bad message block head data.");
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
            StringBuilder sb = new StringBuilder();
            sb.Append("Length:");
            sb.Append(HeadLength);
            sb.Append("+");
            sb.Append(BodyLength);
            sb.Append("\tType:");
            sb.Append(Type.ToString());
            sb.Append("\tPara:");
            switch (Type)
            {
                case MessageBlockType.Handshake:
                    {
                        //sb.AppendLine(((HandshakeType)Parameter).ToString());
                        break;
                    }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates the body by head.
        /// </summary>
        /// <param name="maxBodyLength">Length of the max body.</param>
        public void CreateBodyByHead(int maxBodyLength)
        {
            FromBytes(Head.Buffer, true, maxBodyLength);
        }

        #endregion
    }

    /// <summary>
    /// 消息块类型
    /// </summary>
    public enum MessageBlockType
    {
        /// <summary>
        /// 握手信号
        /// </summary>
        Handshake = 1,
        /// <summary>
        /// 应用程序数据
        /// </summary>
        AppData = 2,
        /// <summary>
        /// 警告
        /// </summary>
        Alert = 3,
        /// <summary>
        /// 关闭通知
        /// </summary>
        CloseNotify = 4,
        /// <summary>
        /// 心跳信号
        /// </summary>
        HeartBeat = 5,
    }
}
