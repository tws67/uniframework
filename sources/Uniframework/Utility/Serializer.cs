using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

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
                throw new UniframeworkException("���л����� \"obj\" ����Ϊ��");

            using (MemoryStream stream = new MemoryStream()) {
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
            using (MemoryStream stream = new MemoryStream(compressor.Decompress(data))) {
                return (T)bf.Deserialize(stream);
            }
        }
    }
}
