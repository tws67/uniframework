using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Reflection;
using System.IO;
using System.Security;
using System.Security.Cryptography;
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
    /// ϵͳ��ȫ�����࣬�������һ������ݼӽ��ܲ���
    /// </summary>
    public static class SecurityUtility
    {
        #region DES��/���ܷ���

        /// <summary>
        /// ��������Hash����
        /// </summary>
        /// <param name="valueToHash">�������</param>
        /// <returns>Hash���е�Base64����</returns>
        public static string HashObject(object valueToHash)
        {
            if (valueToHash == null) valueToHash = typeof(void);
            SHA1 hashingAlgorithm = new SHA1CryptoServiceProvider();
            //XmlSerializer serializer = new XmlSerializer(typeof(object));
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    serializer.Serialize(stream, valueToHash);
            //    return Convert.ToBase64String(hashingAlgorithm.ComputeHash(stream.ToArray()));
            //}
            //byte[] ba = new byte[256];
            //string str = valueToHash.ToString().GetHashCode() +"75D6327AE1294b2dA39C1DB1B7946DC0";
            //int i = 0;
            //foreach (char c in str)
            //{
            //    ba[i++] = (byte)c;
            //}
            //return Convert.ToBase64String(hashingAlgorithm.ComputeHash(ba));

            //MBinaryFormatter bf = new MBinaryFormatter();
            BinaryFormatter bf = new BinaryFormatter();
            //BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bf.Serialize(stream, valueToHash);
                return Convert.ToBase64String(hashingAlgorithm.ComputeHash(stream.ToArray()));
            }
        }

        /// <summary>
        /// ʹ��DES�㷨�����ݽ��м���
        /// </summary>
        /// <param name="data">�����ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <returns>�������ܵ�����</returns>
        public static byte[] DESEncrypt(byte[] data, string key)
        {
            if (data == null) return null;
            if (data.Length == 0) return data;
            return new GenericSymmetricCryptor(DES.Create()).Encrypto(data, key);
        }

        /// <summary>
        /// ʹ��DES�㷨�����ݽ��н���
        /// </summary>
        /// <param name="data">�����ܵ�����</param>
        /// <param name="key">��Կ</param>
        /// <returns>���ܺ������</returns>
        public static byte[] DESDecrypt(byte[] data, string key)
        {
            if (data == null) return null;
            if (data.Length == 0) return data;
            return new GenericSymmetricCryptor(DES.Create()).Decrypto(data, key);
        }
        
        /// <summary>
        /// ʹ��DES�����㷨�����ݼ���
        /// </summary>
        /// <param name="data">�����ܵ��ַ���</param>
        /// <param name="key">��Կ</param>
        /// <returns>���ܺ���ַ���</returns>
        public static string DESEncrypt(string data, string key)
        {
            byte[] buf = System.Text.UTF8Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(DESEncrypt(buf, key));
        }

        /// <summary>
        /// ʹ��DES�����㷨�����ݽ���
        /// </summary>
        /// <param name="data">�����ܵ��ַ���</param>
        /// <param name="key">��Կ</param>
        /// <returns>���ܺ���ַ���</returns>
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
    /// ͨ�õĶԳƼ����㷨��
    /// </summary>
    internal class GenericSymmetricCryptor
    {
        private SymmetricAlgorithm mobjCryptoService;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="algorithm">�ԳƼ����㷨������RC2, DES, TripleDES�ȵ�</param>
        public GenericSymmetricCryptor(SymmetricAlgorithm algorithm)
        {
            mobjCryptoService = algorithm;
        }
        /// <summary>
        /// �����Կ
        /// </summary>
        /// <returns>��Կ</returns>
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
        /// ��ó�ʼ����IV
        /// </summary>
        /// <returns>��������IV</returns>
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
        /// ���ܷ���
        /// </summary>
        /// <param name="byteIn">�����ܵĴ�</param>
        /// <param name="key">���_</param>
        /// <returns>�������ܵ�����</returns>
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
        /// ���ܷ���
        /// </summary>
        /// <param name="byteIn">�����ܵĴ�</param>
        /// <param name="key">���_</param>
        /// <returns>�������ܵ�����</returns>
        public byte[] Decrypto(byte[] byteIn, string key)
        {
            MemoryStream ms = new MemoryStream(byteIn);
            mobjCryptoService.Key = GetLegalKey(key);
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            return ArrayUtility.ReadAllBytesFromStream(cs);
        }
    }

    #endregion
}
