using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// ϵͳ��ȫ����������
    /// </summary>
    public static class SecurityCommon
    {
        /// <summary>
        /// �������һָ�����ȵ��ֽ�����
        /// </summary>
        /// <param name="byteLength"></param>
        /// <returns></returns>
        public static byte[] GenerateBytes(int byteLength)
        {
            byte[] buff = new Byte[byteLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // ��������ʹ��������ǿ������ֽڽ������
            rng.GetBytes(buff);
            return buff;
        }

        /// <summary>
        /// MD5s the specified org.
        /// </summary>
        /// <param name="org">The org.</param>
        /// <returns></returns>
        public static string Md5(string org)
        {
            byte[] bytes = MD5CryptoServiceProvider.Create().ComputeHash(Encoding.Unicode.GetBytes(org));
            return Convert.ToBase64String(bytes);
        }
    }
}
