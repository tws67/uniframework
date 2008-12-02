using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

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
        /// ѹ������
        /// </summary>
        /// <param name="val">ԭʼ������</param>
        /// <returns>ѹ����������</returns>
        public byte[] Compress(byte[] val)
        {
            byte[] buf;
            //���Բ����������Ƿ񳬹������ֵ
            if (overSize != -1 && val.Length > overSize)
            {
                MemoryStream destStream = new MemoryStream();
                using (DeflateStream compressedStream = new DeflateStream(destStream, CompressionMode.Compress)) {
                    compressedStream.Write(val, 0, val.Length);
                    buf = new byte[destStream.Length + 1];
                    destStream.ToArray().CopyTo(buf, 1);
                }
                buf[0] = 1;//ѹ�����
                return buf;
            }
            else {
                buf = new byte[val.Length + 1];
                val.CopyTo(buf, 1);
                buf[0] = 0;//δѹ�����
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
