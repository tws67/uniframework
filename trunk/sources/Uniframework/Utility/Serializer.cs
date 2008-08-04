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
    /// 对象序列化工具
    /// </summary>
    public class Serializer
    {
        private Compressor compressor = new Compressor();
        private BinaryFormatter bf = new BinaryFormatter();

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>序列化后的字节流数据</returns>
        public byte[] Serialize<T>(T obj)
        {
            if (obj == null)
                throw new Exception("序列化对象 \"obj\" 不能为空");

            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, obj);
                return compressor.Compress(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="data">序列化后的字节流数据</param>
        /// <returns>对象实例</returns>
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
