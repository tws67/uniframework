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
    /// 数据块类
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// 通过数据来初始化一个dataBlcok
        /// </summary>
        /// <param name="data">数据块把这个数组作为数据区，获得控制权，而不是复制数据。</param>
        public DataBlock(byte[] data)
        {
            this.buffer = data;
            this.WriteIndex += data.Length;
            this.ReadIndex = 0;
        }

        /// <summary>
        /// 构造函数，默认创建一个1024大小的缓冲区
        /// </summary>
        public DataBlock()
            :this(1024)
        {
        }

        /// <summary>
        /// 通过数据来初始化一个dataBlcok
        /// </summary>
        /// <param name="data">数据块把这个数组作为数据区，获得控制权，而不是复制数据。</param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public DataBlock(byte[]data, int startIndex, int length)
        {
            this.buffer = data;
            this.ReadIndex = startIndex;
            this.WriteIndex = startIndex+length;
        }

        /// <summary>
        /// 通过byte数组段初始化一个dataBlock
        /// </summary>
        /// <param name="data">数组中的一段</param>
        public DataBlock(ArraySegment<byte> data)
        {
            ArraySegment = data;
        }

        /// <summary>
        /// 获取有效数据的数组段
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> ArraySegment
        {
            get
            {
                ArraySegment<byte> result = new ArraySegment<byte>(this.buffer, ReadIndex, DataLength);
                return result;
            }
            set
            {
                buffer = value.Array;
                ReadIndex = value.Offset;
                WriteIndex = value.Offset + value.Count;
            }
        }

        /// <summary>
        /// 获得一个size大小的数据块，数据块的内容为空。
        /// </summary>
        /// <param name="size"></param>
        public DataBlock(int size)
        {
            this.buffer = new byte[size];
        }

        byte[] buffer;

        /// <summary>
        ///  数据块原始数据缓冲区
        /// </summary>
        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        int writeIndex;

        /// <summary>
        /// 数据写指针
        /// </summary>
        public int WriteIndex
        {
            get { return writeIndex; }
            set { writeIndex = value; }
        }

        int readIndex;

        /// <summary>
        /// 数据读指针
        /// </summary>
        public int ReadIndex
        {
            get { return readIndex; }
            set
            {
                if(value<0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (ReadIndex > WriteIndex)
                {
                    throw new ArgumentOutOfRangeException("读指针不能超过写指针。");
                }

                readIndex = value;
            }
        }

        /// <summary>
        /// 复制有效数据到新的数组
        /// </summary>
        public byte[] ToArray()
        {
            byte[] result = new byte[DataLength];
            System.Array.Copy(buffer, ReadIndex, result, 0, DataLength);
            return result;
        }

        /// <summary>
        /// 数据块长度(写指针与读指针之间的距离)
        /// </summary>
        public int DataLength
        {
            get { return WriteIndex - ReadIndex;}
        }

        /// <summary>
        /// 剩余缓冲区长度
        /// </summary>
        public int WritableLength
        {
            get { return buffer.Length - WriteIndex; }
        }

        /// <summary>
        /// 缓冲区总长度
        /// </summary>
        public int Length
        {
            get { return buffer.Length;  }
        }

        /// <summary>
        /// 判断数据块是否已满
        /// </summary>
        public bool IsFull
        {
            get
            {
                return (WritableLength == 0);
            }
        }

        /// <summary>
        /// 从新设定数据块的大小
        /// </summary>
        /// <param name="newSize"></param>
        public void Resize(int newSize)
        {
            System.Array.Resize<byte>(ref buffer, newSize);
            
            ReadIndex = 0;

            Reset();
        }

        /// <summary>
        /// 重置数据块到初始状态
        /// </summary>
        public void Reset()
        {
            WriteIndex = ReadIndex = 0;
        }

        /// <summary>
        /// 添加一个字节数据
        /// </summary>
        /// <param name="b"></param>
        public void Add(byte b)
        {
            buffer[WriteIndex] = b;
            WriteIndex++;
        }

        /// <summary>
        /// 添加一个数组到数据区
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">需要添加的数据在数组中的起始位置</param>
        /// <param name="count">需要添加的数据的个数</param>
        public void Add( byte[] array,int startIndex, int count)
        {
            for(int i=0; i<count; i++)
            {
                Add(array[i+startIndex]);
            }
        }

        /// <summary>
        /// 合并两个数据块
        /// </summary>
        /// <param name="lhs">左边操作对象</param>
        /// <param name="rhs">右边操作对象</param>
        /// <returns></returns>
        public static DataBlock operator +(DataBlock lhs, DataBlock rhs)
        {
            if (lhs == null && rhs == null)
                return null;
            else if (lhs == null)
            {
                DataBlock result = new DataBlock(rhs.DataLength);
                result.Add(rhs.buffer, rhs.ReadIndex, rhs.DataLength);
                return result;
            }
            else if (rhs == null)
            {
                DataBlock result = new DataBlock(lhs.DataLength);
                result.Add(lhs.buffer, lhs.ReadIndex, lhs.DataLength);
                return result;
            }
            else
            {
                DataBlock result = new DataBlock(lhs.DataLength + rhs.DataLength);

                result.Add(lhs.buffer, lhs.ReadIndex, lhs.DataLength);
                result.Add(rhs.buffer, rhs.ReadIndex, rhs.DataLength);

                return result;
            }
        }
    }
}
