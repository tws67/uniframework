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
    /// 基于文本命令的回话基类
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
        /// 文本所使用的编码
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// 命令行分隔字符串
        /// </summary>
        public string[] NewLines
        {
            get
            {
                if (newLines == null || newLines.Length == 0)
                {
                    throw new TcpException("没有设置命令行分隔字符串");
                }
                return newLines;
            }

            set 
            {
                if (value == null || value.Length == 0)
                {
                    throw new TcpException("分隔字符串数组不能为空或者长度不能为0");
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
        /// 接收到readCount个数据。
        /// </summary>
        /// <param name="readCount">数据读取的个数</param>
        protected internal override void ReceivedData(int readCount)
        {
            //线程安全提醒：不能在多线程中调用
            this.Buffer.WriteIndex += readCount;
            //添加上次为使用的数据，重新解析
            cmdData += Buffer;
            //重用数据区
            Buffer.Reset();

            //使用规定的字符集解析字符
            string rawCmd = Encoding.GetString(cmdData.Buffer, cmdData.ReadIndex, cmdData.DataLength);

            //防止溢出攻击，字符串不能无限制的增加
            if(rawCmd.Length > MaxCommandLength)
            {
                throw new NetException("命令太长");
            }

            int length;
            string[] cmdArray = StringEx.Split(rawCmd, NewLines,out length);

            foreach (string cmd in cmdArray)
            {
                //线程安全
                EventHandler<TEventArgs<string>> temp = ReceivedCommand;
                if (temp != null)
                {
                    temp(this, new TEventArgs<string>(cmd));
                }
            }

            //获得已经使用的字符串的数据长度，然后设置为已经读取
            cmdData.ReadIndex += Encoding.GetByteCount(rawCmd.Substring(0, length));
        }
    }
}
