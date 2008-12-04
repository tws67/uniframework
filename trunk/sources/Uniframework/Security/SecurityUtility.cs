using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 系统安全工具类，用于完成一般的数据加解密操作
    /// </summary>
    public static class SecurityUtility
    {
        #region DES加/解密方法

        /// <summary>
        /// 计算对象的Hash数列
        /// </summary>
        /// <param name="valueToHash">处理对象</param>
        /// <returns>Hash数列的Base64编码</returns>
        public static string HashObject(object valueToHash)
        {
            if (valueToHash == null) valueToHash = typeof(void);
            SHA1 hashingAlgorithm = new SHA1CryptoServiceProvider();

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, valueToHash);
                return Convert.ToBase64String(hashingAlgorithm.ComputeHash(stream.ToArray()));
            }
        }

        /// <summary>
        /// 使用DES算法对数据进行加密
        /// </summary>
        /// <param name="data">待加密的数据</param>
        /// <param name="key">密钥</param>
        /// <returns>经过加密的数据</returns>
        public static byte[] DESEncrypt(byte[] data, string key)
        {
            if (data == null) return null;
            if (data.Length == 0) return data;
            return new GenericSymmetricCryptor(DES.Create()).Encrypto(data, key);
        }

        /// <summary>
        /// 使用DES算法对数据进行解密
        /// </summary>
        /// <param name="data">待解密的数据</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的数据</returns>
        public static byte[] DESDecrypt(byte[] data, string key)
        {
            if (data == null) return null;
            if (data.Length == 0) return data;
            return new GenericSymmetricCryptor(DES.Create()).Decrypto(data, key);
        }
        
        /// <summary>
        /// 使用DES加密算法对数据加密
        /// </summary>
        /// <param name="data">欲加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt(string data, string key)
        {
            byte[] buf = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt(buf, key));
        }

        /// <summary>
        /// 使用DES加密算法对数据解密
        /// </summary>
        /// <param name="data">欲解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt(string data, string key)
        {
            byte[] buf = Convert.FromBase64String(data);
            buf = DESDecrypt(buf, key);
            return UTF8Encoding.UTF8.GetString(buf, 0, buf.Length);
        }

        #endregion
    }

    #region GenericSymmetricCryptor

    /// <summary>
    /// 通用的对称加密算法类
    /// </summary>
    internal class GenericSymmetricCryptor
    {
        private SymmetricAlgorithm mobjCryptoService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="algorithm">对称加密算法，比如RC2, DES, TripleDES等等</param>
        public GenericSymmetricCryptor(SymmetricAlgorithm algorithm)
        {
            mobjCryptoService = algorithm;
        }
        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey(string key)
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="byteIn">待加密的串</param>
        /// <param name="key">密鈅</param>
        /// <returns>经过加密的数据</returns>
        public byte[] Encrypto(byte[] byteIn, string key)
        {
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey(key);
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(byteIn, 0, byteIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="byteIn">待解密的串</param>
        /// <param name="key">密鈅</param>
        /// <returns>经过解密的数据</returns>
        public byte[] Decrypto(byte[] byteIn, string key)
        {
            MemoryStream ms = new MemoryStream(byteIn);
            mobjCryptoService.Key = GetLegalKey(key);
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            return ArrayHelper.ReadAllBytesFromStream(cs);
        }
    }

    #endregion
}
