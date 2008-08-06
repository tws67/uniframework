using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// RSA加/解密服务
    /// </summary>
    public class RSACryptServiceBase : IRSACryptService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RSACryptServiceBase"/> class.
        /// </summary>
        protected RSACryptServiceBase()
        {
        }

        private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        #region IRSACryptService 成员

        /// <summary>
        /// Decrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <param name="pvk">The PVK.</param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] inBuffer, string pvk)
        {
            rsa.FromXmlString(pvk);
            return rsa.Decrypt(inBuffer, false);
        }

        /// <summary>
        /// Encrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <param name="puk">The puk.</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] inBuffer, string puk)
        {
            rsa.FromXmlString(puk);
            return rsa.Encrypt(inBuffer, false);
        }

        #endregion
    }


}
