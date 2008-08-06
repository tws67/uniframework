using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 系统安全公共服务类
    /// </summary>
    public static class SecurityCommon
    {
        /// <summary>
        /// 随机生成一指定长度的字节数组
        /// </summary>
        /// <param name="byteLength"></param>
        /// <returns></returns>
        public static byte[] GenerateBytes(int byteLength)
        {
            byte[] buff = new Byte[byteLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // 该数组已使用密码增强的随机字节进行填充
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
