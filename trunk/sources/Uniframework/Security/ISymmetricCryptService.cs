using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISymmetricCryptService
    {
        /// <summary>
        /// Encrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <returns></returns>
        byte[] Encrypt(ArraySegment<byte> inBuffer);
        /// <summary>
        /// Decrypts the specified in buffer.
        /// </summary>
        /// <param name="inBuffer">The in buffer.</param>
        /// <returns></returns>
        byte[] Decrypt(ArraySegment<byte> inBuffer);
        /// <summary>
        /// Gets or sets the SA.
        /// </summary>
        /// <value>The SA.</value>
        SymmetricAlgorithm SA { get; set; }
    }
}
