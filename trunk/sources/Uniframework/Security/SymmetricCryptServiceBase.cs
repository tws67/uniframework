using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class SymmetricCryptServiceBase : ISymmetricCryptService
    {
        SymmetricAlgorithm sa;

        /// <summary>
        /// Gets or sets the SA.
        /// </summary>
        /// <value>The SA.</value>
        public SymmetricAlgorithm SA
        {
            get { return sa; }
            set { sa = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricCryptServiceBase"/> class.
        /// </summary>
        public SymmetricCryptServiceBase()
        {
            SA = new TripleDESCryptoServiceProvider();

            SA.Key = SecurityCommon.GenerateBytes(SA.KeySize / 8);

            SA.IV = SecurityCommon.GenerateBytes(SA.IV.Length);

            sa.Mode = CipherMode.CBC;
            sa.Padding = PaddingMode.ISO10126;
        }

        #region ISymmetricCryptService ≥…‘±

        /// <summary>
        /// Encrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <returns></returns>
        public byte[] Encrypt(ArraySegment<byte> inBuffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    //sa.Padding = padding;
                    cs.Write(inBuffer.Array, inBuffer.Offset, inBuffer.Count);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }

            }
        }

        /// <summary>
        /// Decrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <returns></returns>
        public byte[] Decrypt(ArraySegment<byte> inBuffer)
        {
            using (MemoryStream ms = new MemoryStream(inBuffer.Array, inBuffer.Offset, inBuffer.Count))
            {
                //sa.Padding = padding;

                using (CryptoStream cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Read))
                using (BinaryReader b = new BinaryReader(cs))
                {
                    //ms.Position = 0;
                    //sa.Padding = padding;
                    return b.ReadBytes(8192);
                }

            }
        }

        #endregion
    }
}
