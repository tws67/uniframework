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
    /// ���ݿ���
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// ͨ����������ʼ��һ��dataBlcok
        /// </summary>
        /// <param name="data">���ݿ�����������Ϊ����������ÿ���Ȩ�������Ǹ������ݡ�</param>
        public DataBlock(byte[] data)
        {
            this.buffer = data;
            this.WriteIndex += data.Length;
            this.ReadIndex = 0;
        }

        /// <summary>
        /// ���캯����Ĭ�ϴ���һ��1024��С�Ļ�����
        /// </summary>
        public DataBlock()
            :this(1024)
        {
        }

        /// <summary>
        /// ͨ����������ʼ��һ��dataBlcok
        /// </summary>
        /// <param name="data">���ݿ�����������Ϊ����������ÿ���Ȩ�������Ǹ������ݡ�</param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public DataBlock(byte[]data, int startIndex, int length)
        {
            this.buffer = data;
            this.ReadIndex = startIndex;
            this.WriteIndex = startIndex+length;
        }

        /// <summary>
        /// ͨ��byte����γ�ʼ��һ��dataBlock
        /// </summary>
        /// <param name="data">�����е�һ��</param>
        public DataBlock(ArraySegment<byte> data)
        {
            ArraySegment = data;
        }

        /// <summary>
        /// ��ȡ��Ч���ݵ������
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
        /// ���һ��size��С�����ݿ飬���ݿ������Ϊ�ա�
        /// </summary>
        /// <param name="size"></param>
        public DataBlock(int size)
        {
            this.buffer = new byte[size];
        }

        byte[] buffer;

        /// <summary>
        ///  ���ݿ�ԭʼ���ݻ�����
        /// </summary>
        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        int writeIndex;

        /// <summary>
        /// ����дָ��
        /// </summary>
        public int WriteIndex
        {
            get { return writeIndex; }
            set { writeIndex = value; }
        }

        int readIndex;

        /// <summary>
        /// ���ݶ�ָ��
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
                    throw new ArgumentOutOfRangeException("��ָ�벻�ܳ���дָ�롣");
                }

                readIndex = value;
            }
        }

        /// <summary>
        /// ������Ч���ݵ��µ�����
        /// </summary>
        public byte[] ToArray()
        {
            byte[] result = new byte[DataLength];
            System.Array.Copy(buffer, ReadIndex, result, 0, DataLength);
            return result;
        }

        /// <summary>
        /// ���ݿ鳤��(дָ�����ָ��֮��ľ���)
        /// </summary>
        public int DataLength
        {
            get { return WriteIndex - ReadIndex;}
        }

        /// <summary>
        /// ʣ�໺��������
        /// </summary>
        public int WritableLength
        {
            get { return buffer.Length - WriteIndex; }
        }

        /// <summary>
        /// �������ܳ���
        /// </summary>
        public int Length
        {
            get { return buffer.Length;  }
        }

        /// <summary>
        /// �ж����ݿ��Ƿ�����
        /// </summary>
        public bool IsFull
        {
            get
            {
                return (WritableLength == 0);
            }
        }

        /// <summary>
        /// �����趨���ݿ�Ĵ�С
        /// </summary>
        /// <param name="newSize"></param>
        public void Resize(int newSize)
        {
            System.Array.Resize<byte>(ref buffer, newSize);
            
            ReadIndex = 0;

            Reset();
        }

        /// <summary>
        /// �������ݿ鵽��ʼ״̬
        /// </summary>
        public void Reset()
        {
            WriteIndex = ReadIndex = 0;
        }

        /// <summary>
        /// ���һ���ֽ�����
        /// </summary>
        /// <param name="b"></param>
        public void Add(byte b)
        {
            buffer[WriteIndex] = b;
            WriteIndex++;
        }

        /// <summary>
        /// ���һ�����鵽������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="startIndex">��Ҫ��ӵ������������е���ʼλ��</param>
        /// <param name="count">��Ҫ��ӵ����ݵĸ���</param>
        public void Add( byte[] array,int startIndex, int count)
        {
            for(int i=0; i<count; i++)
            {
                Add(array[i+startIndex]);
            }
        }

        /// <summary>
        /// �ϲ��������ݿ�
        /// </summary>
        /// <param name="lhs">��߲�������</param>
        /// <param name="rhs">�ұ߲�������</param>
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
