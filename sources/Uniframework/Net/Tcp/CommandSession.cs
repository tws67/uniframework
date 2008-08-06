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
    /// �����ı�����Ļػ�����
    /// </summary>
    public class CommandSession:TcpSession
    {
        private Encoding encoding;
        private string[] newLines;
        private int maxCommandLength = 64*1024;

        /// <summary>
        /// Gets or sets the length of the max command.
        /// </summary>
        /// <value>The length of the max command.</value>
        public int MaxCommandLength
        {
            get { return maxCommandLength; }
            set { maxCommandLength = value; }
        }
        
        private DataBlock cmdData = new DataBlock();

        /// <summary>
        /// �ı���ʹ�õı���
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// �����зָ��ַ���
        /// </summary>
        public string[] NewLines
        {
            get
            {
                if (newLines == null || newLines.Length == 0)
                {
                    throw new TcpException("û�����������зָ��ַ���");
                }
                return newLines;
            }

            set 
            {
                if (value == null || value.Length == 0)
                {
                    throw new TcpException("�ָ��ַ������鲻��Ϊ�ջ��߳��Ȳ���Ϊ0");
                }

                newLines = value; 
            }
        }

        /// <summary>
        /// Occurs when [received command].
        /// </summary>
        public event EventHandler<TEventArgs<string>> ReceivedCommand;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("CMD-S:{0:0000}", ID);
        }

        /// <summary>
        /// ���յ�readCount�����ݡ�
        /// </summary>
        /// <param name="readCount">���ݶ�ȡ�ĸ���</param>
        protected internal override void ReceivedData(int readCount)
        {
            //�̰߳�ȫ���ѣ������ڶ��߳��е���
            this.Buffer.WriteIndex += readCount;
            //����ϴ�Ϊʹ�õ����ݣ����½���
            cmdData += Buffer;
            //����������
            Buffer.Reset();

            //ʹ�ù涨���ַ��������ַ�
            string rawCmd = Encoding.GetString(cmdData.Buffer, cmdData.ReadIndex, cmdData.DataLength);

            //��ֹ����������ַ������������Ƶ�����
            if(rawCmd.Length > MaxCommandLength)
            {
                throw new NetException("����̫��");
            }

            int length;
            string[] cmdArray = StringEx.Split(rawCmd, NewLines,out length);

            foreach (string cmd in cmdArray)
            {
                //�̰߳�ȫ
                EventHandler<TEventArgs<string>> temp = ReceivedCommand;
                if (temp != null)
                {
                    temp(this, new TEventArgs<string>(cmd));
                }
            }

            //����Ѿ�ʹ�õ��ַ��������ݳ��ȣ�Ȼ������Ϊ�Ѿ���ȡ
            cmdData.ReadIndex += Encoding.GetByteCount(rawCmd.Substring(0, length));
        }
    }
}
