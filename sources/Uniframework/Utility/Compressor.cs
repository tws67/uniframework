using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 内部数据压缩/解压缩类
    /// </summary>
    public class Compressor
    {
        private int overSize = 1024; // 默认的压缩处理阀值为1kb字节以上

        /// <summary>
        /// 构造函数
        /// </summary>
        public Compressor() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="overSize">压缩处理阀值</param>
        public Compressor(int overSize)
            : this()
        {
            this.overSize = overSize;
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="val">原始数据流</param>
        /// <returns>压缩后数据流</returns>
        public byte[] Compress(byte[] val)
        {
            byte[] buf;
            //测试参数流长度是否超过定义的值
            if (overSize != -1 && val.Length > overSize)
            {
                MemoryStream destStream = new MemoryStream();
                using (DeflateStream compressedStream = new DeflateStream(destStream, CompressionMode.Compress)) {
                    compressedStream.Write(val, 0, val.Length);
                    buf = new byte[destStream.Length + 1];
                    destStream.ToArray().CopyTo(buf, 1);
                }
                buf[0] = 1;//压缩标记
                return buf;
            }
            else {
                buf = new byte[val.Length + 1];
                val.CopyTo(buf, 1);
                buf[0] = 0;//未压缩标记
                return buf;
            }
        }

        /// <summary>
        /// 解压字节流数据
        /// </summary>
        /// <param name="val">待处理的字节流数据</param>
        /// <returns>返回解压后的数据</returns>
        public byte[] Decompress(byte[] val)
        {
            if (val[0] == 1)
            {
                DeflateStream compressedStream = new DeflateStream(new MemoryStream(UnwrapData(val)), CompressionMode.Decompress);
                return ArrayUtility.ReadAllBytesFromStream(compressedStream);
            }
            else
            {
                return UnwrapData(val);
            }
        }

        /// <summary>
        /// 去除数据内部的压缩标志位
        /// </summary>
        /// <param name="wrapData">数据流</param>
        /// <returns>原始的数据流</returns>
        private byte[] UnwrapData(byte[] data)
        {
            byte[] buffer = new byte[data.Length - 1];
            Array.Copy(data, 1, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
