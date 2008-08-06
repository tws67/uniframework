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
using System.Diagnostics;

namespace Uniframework.Net
{
    /// <summary>
    /// 安全通信服务器类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class SecureServerBase<TSession> : MessageBlockServer<TSession>
        where TSession : SecureSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureServerBase&lt;TSession&gt;"/> class.
        /// </summary>
        public SecureServerBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureServerBase&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public SecureServerBase(int port)
            : base(port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureServerBase&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="capacity">The capacity.</param>
        public SecureServerBase(int port, int capacity)
            : base(port, capacity)
        {
        }

        RSAKey key = RSAKey.CreateKey();

        /// <summary>
        /// Called when [received message block].
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="mb">The mb.</param>
        protected override void OnReceivedMessageBlock(TSession session, MessageBlock mb)
        {
            switch (mb.Type)
            {
                case MessageBlockType.AppData:
                    {
                        if (!session.IsBuildSecureConnection)
                        {
                            throw new NotBuildSecureConnectionException();
                        }
                        byte[] data = session.SymmetricCryptService.Decrypt(mb.Body.ArraySegment);
                        OnReceivedData(session, new DataBlock(data));
                        break;
                    }

                case MessageBlockType.Handshake:
                    {
                        HandleHandshake(session, (HandshakeType)mb.Parameter, mb);
                        break;
                    }

                case MessageBlockType.CloseNotify:
                    {
                        session.Close();
                        break;
                    }

                case MessageBlockType.Alert:
                    {
                        break;
                    }

                case MessageBlockType.HeartBeat:
                    {
                        Debug.Assert(false);//已经在MessageServer中处理了，不会出现在这里
                        break;
                    }
            }
        }

        private void HandleHandshake(TSession session, HandshakeType handshakeType, MessageBlock mb)
        {
            switch (handshakeType)
            {
                case HandshakeType.ClientHello:
                    {
                        MessageBlock mb1 = new MessageBlock(MessageBlockType.Handshake,
                           (int)HandshakeType.ServerHello);
                        base.Send(session, mb1);
                        session.Handshake = HandshakeType.ServerHello;

                        NetDebuger.PrintDebugMessage(session, "ClientHello");
                        break;
                    }

                case HandshakeType.ClientKeyExchange:
                    {
                        session.CheckPrePhase(HandshakeType.ServerHello);
                        session.RemotePublicKey = Encoding.Unicode.GetString(mb.Body.Buffer);

                        NetDebuger.PrintDebugMessage(session, "Client Public Key:" + session.RemotePublicKey);

                        MessageBlock mb2 = new MessageBlock(
                            MessageBlockType.Handshake,
                            (int)HandshakeType.ServerKeyExchange,
                            new DataBlock(Encoding.Unicode.GetBytes(key.PublicKey)));

                        base.Send(session, mb2);
                        session.Handshake = HandshakeType.ServerKeyExchange;
                        NetDebuger.PrintDebugMessage(session, "ClientKeyExchange");
                        break;
                    }

                case HandshakeType.ClientSymmetricKey:
                    {
                        session.CheckPrePhase(HandshakeType.ServerKeyExchange);
                        byte[] key = Singleton<RSACryptServiceBase>.Instance.Decrypt(
                            mb.Body.Buffer, this.key.XmlString);
                        session.SymmetricCryptService.SA.Key = key;

                        NetDebuger.PrintDebugMessage(session, "Client SA Key:" + Convert.ToBase64String(key));

                        MessageBlock getKeymb = new MessageBlock(MessageBlockType.Handshake,
                                (int)HandshakeType.ServerGetSymmetricKey);
                        base.Send(session, getKeymb);

                        session.Handshake = HandshakeType.ServerGetSymmetricKey;
                        NetDebuger.PrintDebugMessage(session, "ClientSymmetrickey");
                        break;
                    }

                case HandshakeType.ClientSymmetricIV:
                    {
                        session.CheckPrePhase(HandshakeType.ServerGetSymmetricKey);
                        byte[] iv = Singleton<RSACryptServiceBase>.Instance.Decrypt(
                            mb.Body.Buffer, this.key.XmlString);
                        session.SymmetricCryptService.SA.IV = iv;

                        NetDebuger.PrintDebugMessage(session, "Client SA IV:" + Convert.ToBase64String(iv));

                        MessageBlock getIVmb = new MessageBlock(MessageBlockType.Handshake,
                                (int)HandshakeType.ServerGetSymmetricIV);
                        base.Send(session, getIVmb);

                        session.Handshake = HandshakeType.ServerGetSymmetricIV;
                        NetDebuger.PrintDebugMessage(session, "ClientSymmetricIV");
                        break;
                    }

                case HandshakeType.ClientFinished:
                    {
                        session.CheckPrePhase(HandshakeType.ServerGetSymmetricIV);
                        MessageBlock mb3 = new MessageBlock(MessageBlockType.Handshake,
                                (int)HandshakeType.ServerFinished);
                        base.Send(session, mb3);
                        session.Handshake = HandshakeType.OK;
                        NetDebuger.PrintDebugMessage(session, "Client Handshake Finished");
                        OnBuildDataConnection(session);
                        NetDebuger.PrintDebugMessage(session, "Build security community channel");
                        break;
                    }
            }
        }

        /// <summary>
        /// 回话已经创建安全数据连接。
        /// 为了及时的接收数据，该函数需要立即返回。
        /// </summary>
        /// <param name="session"></param>
        protected virtual void OnBuildDataConnection(TSession session)
        {
        }

        /// <summary>
        /// 发送消息，消息的内容会被加密传输
        /// </summary>
        /// <param name="session"></param>
        /// <param name="msg"></param>
        public override void Send(TSession session, MessageBlock msg)
        {
            if (!session.IsBuildSecureConnection)
            {
                throw new NotBuildSecureConnectionException();
            }

            if (msg.Body != null & msg.BodyLength != 0) //加密非空的消息数据
            {
                byte[] cryptData = session.SymmetricCryptService.Encrypt(msg.Body.ArraySegment);
                msg = new MessageBlock(msg.Type, msg.Parameter, new DataBlock(cryptData));
            }

            base.Send(session, msg);
        }

    }
}
