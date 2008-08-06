using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Uniframework
{
    /// <summary>
    /// 数据转换工具类
    /// </summary>
    public static class MyConvert
    {
        #region ToString
        /// <summary>
        /// 将byte数组转化为字符串。每个byte之间使用空格分隔
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(byte[] value)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < value.Length; ++i)
            {
                result.AppendFormat("{0:X2}", value[i]);
                if (i != value.Length - 1)
                {
                    result.Append(" "); //add a space
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 将一个byte转化为字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(byte value)
        {
            return string.Format("{0:X2}",value);
        }

        /// <summary>
        /// 将byte数组转化为字符串。每个byte之间使用空格分隔。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addSpace">是否添加空格</param>
        /// <param name="addPreExpress">是否添加前缀0x</param>
        /// <returns></returns>
        public static string ToString(byte[] value,bool addSpace, bool addPreExpress)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < value.Length; ++i)
            {
                if (addPreExpress)
                {
                    result.AppendFormat("0x");
                }

                result.AppendFormat("{0:X2}", value[i]);

                if(addSpace)
                {
                    if (i != value.Length - 1)
                    {
                        result.Append(" "); //add a space
                    }
                }
            }

            return result.ToString();
        }
        #endregion
       
        #region ToUInt32
        public static UInt32 ToUInt32(UInt16 high, UInt16 low)
        {
            UInt32 result = high;
            return (result << 16) | low;
        }

        public static UInt32 ToUInt32(Byte b1, Byte b2, Byte b3, Byte b4)
        {
            return ToUInt32(ToUInt16(b1, b2), ToUInt16(b3, b4));
        }

        public static UInt32 ToUInt32(byte[] value, int pos)
        {
            return ToUInt32(value[pos], value[pos + 1], value[pos + 2], value[pos + 3]);
        }
        #endregion

        #region ToUInt16
        /// <summary>
        /// 将2个字节转化为一个UInt16,高位在前,低位在后
        /// </summary>
        /// <param name="high">高位部分</param>
        /// <param name="low">低位部分</param>
        /// <returns>结果</returns>
        public static UInt16 ToUInt16(Byte high, Byte low)
        {
            UInt16 result = high;
            return  (UInt16)( (result << 8) | low );
        }
        #endregion

        #region ToByteArray
        /// <summary>
        /// 将64位无符号正型转化为数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns>byte数组高位在前,低位在后</returns>
        public static byte[] ToByteArray(UInt64 value)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                unchecked
                {
                    result[7-i] = Convert.ToByte(value & 0x00000000000000FF);
                    if (i != 7)
                     value >>= 8;
                }
            }
            return result;
        }

        public static byte[] ToByteArray(string inString)
        {
            string[] byteStrings;
            byteStrings = inString.Split(" ,;.-".ToCharArray());
            if (byteStrings.Length == 0)
            {
                return new byte[0];
            }

            List<String> validStrings = new List<string>(byteStrings.Length);
            foreach (String org in byteStrings)
            {
                if (org != String.Empty)
                {
                    validStrings.Add(org);
                }
            }

            byte[] bytesOut = new byte[validStrings.Count];
            for (int i = 0; i < validStrings.Count; i++)
            {
                bytesOut[i] = Convert.ToByte((validStrings[i]), 16);
            }

            return bytesOut;
        }

        #endregion

        #region ToUInt64
        public static UInt64 ToUInt64(byte[] value)
        {
            UInt64 result = 0 ;

            for (int i = 0; i < 8; ++i)
            {
                result = result | value[i];

                if(i!=7)
                    result = result << 8;
            }

            return result;
        }


        #endregion
         
        #region ToByte
        /// <summary>
        /// 获得一个16位无符号整数的高位或者低位字节
        /// </summary>
        /// <param name="value">16位无符号整数</param>
        /// <param name="highPartition">true: 高位字节;false:低位字节</param>
        /// <returns>字节值</returns>
        public static Byte ToByte(UInt16 value, bool highPartition)
        {
            Byte result = 0;

            if(highPartition) //Get high partition
            {
                result =(Byte)(value >> 8);
            }
            else //Get low partition
            {
                result = (Byte)(value & 0x00FF);
            }

            return result;

        }
        #endregion

        #region ToUint16Array

        /// <summary>
        /// 将偶数各成员的byte数组转为一个UInt16数组.高位在前,低位在后
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index">起始位置</param>
        /// <param name="length">长度(偶数)</param>
        /// <returns></returns>
        public static UInt16[] ToUint16Array(byte[] value, int index, int length)
        {
            Debug.Assert(length % 2 == 0, "长度应该是一个偶数");

            ushort[] validWordData = new ushort[length / 2];

            for (int i = 0; i < validWordData.Length ; ++i )
            {
                validWordData[i] = MyConvert.ToUInt16(value[index+ 2 * i], value[index +2 * i+1 ]);
            }

            return validWordData;
        }
        
        #endregion

        #region Rotate
        /// <summary>
        /// 旋转一个byte数组的内容
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Rotate(byte[] value)
        {
            for (int i = 0; i < value.Length / 2; ++i)
            {
                byte temp = value[i];
                value[ i ] = value[ value.Length - i - 1];
                value[value.Length - i - 1] = temp;
            }

            return value;
        }
        #endregion

        #region ByteToBCD

        /// <summary>
        /// 字节转化为BCD码
        /// </summary>
        /// <param name="value">需要转化的字节</param>
        /// <param name="highPartition"></param>
        /// <returns></returns>
        public static byte ByteToBCD(byte value)
        {
            return 0;
        }

        /// <summary>
        /// BCD码转为字节
        /// </summary>
        /// <param name="highBCD">字节高位的BCD码</param>
        /// <param name="lowBCD">字节地位的BCD码</param>
        /// <returns></returns>
        public static byte BCDToByte(byte highBCD, byte lowBCD)
        {
            byte highPartition = BCDToByte(highBCD);
            byte lowPartition = BCDToByte(lowBCD);

            return (byte)((highPartition << 4) | lowPartition);
            
        }

        public static byte BCDToByte(byte bcd)
        {
            byte high4bits = (byte)(bcd >> 4);
            byte low4bits = (byte)(bcd & 0x0F);
            return (byte)(high4bits * 10 + low4bits);
        }

        #endregion

        #region ToBytes
        /// <summary>
        /// 将64位无符号正型转化为数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns>byte数组高位在前,低位在后</returns>
        public static byte[] ToBytes(UInt64 value)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                unchecked
                {
                    result[7 - i] = Convert.ToByte(value & 0x00000000000000FF);
                    if (i != 7)
                        value >>= 8;
                }
            }
            return result;
        }

        public static byte[] ToBytes(int value)
        {
            return ToBytes((uint)value);
        }

        public static byte[] ToBytes(UInt32 value)
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 4; ++i)
            {
                result[3 - i] = Convert.ToByte(value & 0x000000FF);
                if (i != 3)
                {
                    value >>= 8;
                }
            }

            return result;
        }


        #endregion

    }
}
