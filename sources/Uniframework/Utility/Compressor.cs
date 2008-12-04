using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Uniframework
{
    /// <summary>
    /// �ڲ�����ѹ��/��ѹ����
    /// </summary>
    public class Compressor
    {
        private int overSize = 1024; // Ĭ�ϵ�ѹ������ֵΪ1kb�ֽ�����

        /// <summary>
        /// ���캯��
        /// </summary>
        public Compressor() { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="overSize">ѹ������ֵ</param>
        public Compressor(int overSize)
            : this()
        {
            this.overSize = overSize;
        }

        /// <summary>
        /// ѹ�����ݣ����ѹ����ֵΪ-1��ʾ��������������ѹ������
        /// </summary>
        /// <param name="val">ԭʼ������</param>
        /// <returns>ѹ����������</returns>
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
                    buf[0] = 1; // ��ѹ����־
                    return buf;
                }
            }
            else {
                buf = new byte[val.Length + 1];
                val.CopyTo(buf, 1);
                buf[0] = 0; // δѹ����־
                return buf;
            }
        }

        /// <summary>
        /// ��ѹ�ֽ�������
        /// </summary>
        /// <param name="val">��������ֽ�������</param>
        /// <returns>���ؽ�ѹ�������</returns>
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
        /// ȥ�������ڲ���ѹ����־λ
        /// </summary>
        /// <param name="wrapData">������</param>
        /// <returns>ԭʼ��������</returns>
        private byte[] UnwrapData(byte[] data)
        {
            byte[] buffer = new byte[data.Length - 1];
            Array.Copy(data, 1, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
