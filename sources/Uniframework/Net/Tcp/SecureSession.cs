// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

using Uniframework.Security;

namespace Uniframework.Net
{
    /// <summary>
    /// ��ȫͨ�ŻỰ��
    /// </summary>
    public class SecureSession : MessageBlockSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureSession"/> class.
        /// </summary>
        public SecureSession()
        {
            SymmetricCryptService = new SymmetricCryptServiceBase();
        }

        HandshakeType handshake;
        /// <summary>
        /// Gets or sets the handshake.
        /// </summary>
        /// <value>The handshake.</value>
        public HandshakeType Handshake
        {
            get { return handshake; }
            set { handshake = value; }
        }

        /// <summary>
        /// �����ֵ�ʱ��ʹ�÷ǶԳƼ��ܷ�����ͨ��Զ�˵Ĺ���
        /// </summary>
        string publicKey;

        /// <summary>
        /// Gets or sets the remote public key.
        /// </summary>
        /// <value>The remote public key.</value>
        public string RemotePublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        /// <summary>
        /// �Ƿ��Ѿ�������ȫ����
        /// </summary>
        public bool IsBuildSecureConnection
        {
            get
            {
                return Handshake == HandshakeType.OK;
            }
        }

        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("SMB-S::{0:0000}", ID);
        }

        /// <summary>
        /// Checks the pre phase.
        /// </summary>
        /// <param name="type">The type.</param>
        public void CheckPrePhase(HandshakeType type)
        {
            if (Handshake != type)
            {
                throw new NetSecureException("����İ�ȫͨ�Ž���˳��");
            }
        }

        ISymmetricCryptService symmetricCryptService;

        /// <summary>
        /// ͨ�Ŷ�ʹ�õĶԳƼ��ܷ���(����ڷ�������ʹ�ã�Ӧ���ÿͻ��˵�key��IV��������)
        /// </summary>
        public ISymmetricCryptService SymmetricCryptService
        {
            get { return symmetricCryptService; }
            set { symmetricCryptService = value; }
        }


    }
}
