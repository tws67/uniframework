using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Uniframework
{
    /// <summary>
    /// 64λ��GUID����128λ��GUID��ͬ�������Ժ�һ��64λ�޷�����������ת����
    /// �����ڱȽ϶̵�GUID���ϡ�
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
        /// �����µ�GUID
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
        /// ���16���ַ���GUID
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return id;
        }

        /// <summary>
        /// ���8���ַ���GUID
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            return ToString().Substring(0,8);
        }
        
    }
}
