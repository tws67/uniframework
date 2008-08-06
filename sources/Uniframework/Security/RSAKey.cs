using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Uniframework.Security
{
    /// <summary>
    /// RSA key
    /// </summary>
    public class RSAKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RSAKey"/> class.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        public RSAKey(string xmlString)
        {
            this.xmlString = xmlString;
        }

        /// <summary>
        /// Creates the key.
        /// </summary>
        /// <returns></returns>
        static public RSAKey CreateKey()
        {
            RSAKeyValue rsa = new RSAKeyValue();
            return new RSAKey(rsa.Key.ToXmlString(true));
        }

        private string xmlString;

        /// <summary>
        /// Gets the XML string.
        /// </summary>
        /// <value>The XML string.</value>
        public string XmlString
        {
            get { return xmlString; }
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <value>The public key.</value>
        public string PublicKey
        {
            get
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlString);
                return rsa.ToXmlString(false);
            }
        }
    }
}
