using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// RSA加/解密服务
    /// </summary>
    public interface IRSACryptService
    {
        /// <summary>
        /// Decrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <param name="pvk">The PVK.</param>
        /// <returns></returns>
        byte[] Decrypt(byte[] inBuffer, string pvk);
        /// <summary>
        /// Encrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <param name="puk">The puk.</param>
        /// <returns></returns>
        byte[] Encrypt(byte[] inBuffer, string puk);

    }
}
