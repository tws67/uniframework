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
    /// ��Ϣ����
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
        /// ����һ����MessageBlock,ֻ����ͷ��Ϣ
        /// </summary>
        public MessageBlock()
        {
            Head = new DataBlock(HeadLength);
        }

        /// <summary>
        /// ����һ������Ӧ�����ݵ���Ϣ
        /// </summary>
        /// <param name="data">Ӧ������</param>
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
        /// ����һ���ض����͵�MessageBlok
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
        /// ��Ϣ����
        /// </summary>
        public MessageBlockType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public int Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        /// <summary>
        /// ��Ϣ���ݲ���
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
        /// ��Ϣͷ����
        /// </summary>
        public DataBlock Head
        {
            get { return head; }
            set { head = value; }
        }

        /// <summary>
        ///  ��Ϣ���С
        /// </summary>
        public int BodyLength
        {
            get
            {
                return bodyLength;
            }
        }

        /// <summary>
        /// ��Ϣ�ܵĴ�С
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
        /// ������Ϣ���ã����ɱ���ͷ��Ϣ
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
            //��ӳ�����Ϣ
            Array.Copy(lengthBytes, 0, result, index, 4);
            index += 4;
            //���������Ϣ
            Array.Copy(typeBytes, 0, result, index, 4);
            index += 4;
            //��Ӳ�����Ϣ
            Array.Copy(parameterBytes, 0, result, index, 4);

            //��չ��Ϣ��δʹ�ã���Ϊ0
            index += 8;

            this.Head.WriteIndex = HeadLength;
        }

        /// <summary>
        /// �����ݵõ�һ����Ϣ
        /// </summary>
        /// <param name="data">������Ϣ������</param>
        /// <param name="getHeadData">�Ƿ�õ�ͷ��Ϣ</param>
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
    /// ��Ϣ������
    /// </summary>
    public enum MessageBlockType
    {
        /// <summary>
        /// �����ź�
        /// </summary>
        Handshake = 1,
        /// <summary>
        /// Ӧ�ó�������
        /// </summary>
        AppData = 2,
        /// <summary>
        /// ����
        /// </summary>
        Alert = 3,
        /// <summary>
        /// �ر�֪ͨ
        /// </summary>
        CloseNotify = 4,
        /// <summary>
        /// �����ź�
        /// </summary>
        HeartBeat = 5,
    }
}
