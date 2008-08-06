using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Uniframework
{
    /// <summary>
    /// 64位的GUID。与128位的GUID不同，它可以和一个64位无符号整数进行转换。
    /// 适用于比较短的GUID场合。
    /// </summary>
    public class ShortGuid
    {
        string id;
        static int seq;
        static Random randomer;

        public ShortGuid(string id)
        {
            this.id = id;
        }

        static ShortGuid()
        {
            randomer = new Random();
            seq = randomer.Next() ;
        }

        /// <summary>
        /// 生成新的GUID
        /// </summary>
        /// <returns></returns>
        public static ShortGuid Create()
        {
            DateTime now = DateTime.Now;
            UInt64 value = (UInt64)now.Ticks;

            byte[] valueBytes = MyConvert.ToByteArray(value);
            
            value = (UInt64)( MyConvert.ToUInt64(valueBytes)*(ulong)randomer.Next()) ;
            value += (ulong)seq;
            seq++;

            valueBytes = MyConvert.ToByteArray(value);

            return new ShortGuid(MyConvert.ToString(valueBytes, false, false).ToUpper());
        }

        /// <summary>
        /// 获得16个字符的GUID
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return id;
        }

        /// <summary>
        /// 获得8个字符的GUID
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            return ToString().Substring(0,8);
        }
        
    }
}
