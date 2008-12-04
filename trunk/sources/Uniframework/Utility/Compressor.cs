using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

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
        /// 压缩数据，如果压缩阀值为-1表示不对数据流进行压缩处理
        /// </summary>
        /// <param name="val">原始数据流</param>
        /// <returns>压缩后数据流</returns>
        public byte[] Compress(byte[] val)
        {
            byte[] buf;
            if (overSize != -1 && val.Length > overSize) {
                using (MemoryStream destStream = new MemoryStream()) {
                    Deflater deflater = new Deflater(Deflater.DEFAULT_COMPRESSION, true);
                    using (DeflaterOutputStream compressStream = new DeflaterOutputStream(destStream, deflater)) {
                        compressStream.Write(val, 0, val.Length);
                        compressStream.Finish();
                        buf = new byte[destStream.Length + 1];
                        destStream.ToArray().CopyTo(buf, 1);
                    }
                    buf[0] = 1; // 已压缩标志
                    return buf;
                }
            }
            else {
                buf = new byte[val.Length + 1];
                val.CopyTo(buf, 1);
                buf[0] = 0; // 未压缩标志
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
            if (val[0] == 1) {
                Inflater inflater = new Inflater(true);
                using (InflaterInputStream decompressStream = new InflaterInputStream(new MemoryStream(UnwrapData(val)), inflater)) {
                    return ArrayHelper.ReadAllBytesFromStream(decompressStream);
                }
            }
            else
                return UnwrapData(val);
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
