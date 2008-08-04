using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Xml.Serialization;
#if PocketPC || WindowsCE
using AsGoodAsItGets.System.Runtime.Serialization;
using AsGoodAsItGets.System.Runtime.Serialization.Formatters.Binary;
using AsGoodAsItGets.System.Runtime.Serialization.Surrogates.Binary;
#else
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Uniframework
{
    /// <summary>
    /// �������л�����
    /// </summary>
    public class Serializer
    {
        private Compressor compressor = new Compressor();
        private BinaryFormatter bf = new BinaryFormatter();

        /// <summary>
        /// ���л�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="obj">����ʵ��</param>
        /// <returns>���л�����ֽ�������</returns>
        public byte[] Serialize<T>(T obj)
        {
            if (obj == null)
                throw new Exception("���л����� \"obj\" ����Ϊ��");

            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, obj);
                return compressor.Compress(stream.ToArray());
            }
        }

        /// <summary>
        /// �����л�����
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="data">���л�����ֽ�������</param>
        /// <returns>����ʵ��</returns>
        public T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            using (MemoryStream stream = new MemoryStream(compressor.Decompress(data)))
            {
                return (T)bf.Deserialize(stream);
            }
        }
    }
}
