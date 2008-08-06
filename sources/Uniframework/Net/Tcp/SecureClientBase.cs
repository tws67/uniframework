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
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;

using Uniframework.Security;

namespace Uniframework.Net
{
    /// <summary>
    /// 安全通讯客户端类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class SecureClientBase<TSession> : MessageBlockClient<TSession>
        where TSession : SecureSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureClientBase&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <param name="listenPort">The listen port.</param>
        public SecureClientBase(string hostNameOrAddress, int listenPort)
            : base(hostNameOrAddress, listenPort)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureClientBase&lt;TSession&gt;"/> class.
        /// </summary>
        public SecureClientBase()
            : base()
        {
        }

        RSAKey key = RSAKey.CreateKey();

        protected override void OnConnectServer()
        {
            //Send handshake message block to build secure connection like SSL
            base.Send(new MessageBlock(MessageBlockType.Handshake, (int)HandshakeType.ClientHello));
            Session.Handshake = HandshakeType.ClientHello;
            NetDebuger.PrintDebugMessage(Session, "SEND ClientHello");
        }

        private void HandleHandshakeType(HandshakeType type, MessageBlock mb)
        {
            switch (type)
            {
                case HandshakeType.ServerHello:
                    {
                        NetDebuger.PrintDebugMessage(Session, "RECV ServerHello");

                        Session.CheckPrePhase(HandshakeType.ClientHello);

                        MessageBlock mb1 = new MessageBlock(MessageBlockType.Handshake,
                            (int)HandshakeType.ClientKeyExchange,
                            new DataBlock(Encoding.Unicode.GetBytes(key.PublicKey)));
                        base.Send(mb1);
                        Session.Handshake = HandshakeType.ClientKeyExchange;
                        NetDebuger.PrintDebugMessage(Session, "SEND ClientKeyExchange");
                    }
                    break;

                case HandshakeType.ServerKeyExchange:
                    {
                        NetDebuger.PrintDebugMessage(Session, "RECV ServerKeyExchange");
                        Session.CheckPrePhase(HandshakeType.ClientKeyExchange);
                        Session.RemotePublicKey = Encoding.Unicode.GetString(mb.Body.Buffer);
                        //Get Crypt key data
                        byte[] keyData = Singleton<RSACryptServiceBase>.Instance.Encrypt
                            (Session.SymmetricCryptService.SA.Key, Session.RemotePublicKey);

                        //Send Client Symmetric key
                        MessageBlock skmb = new MessageBlock(MessageBlockType.Handshake,
                            (int)HandshakeType.ClientSymmetricKey,
                            new DataBlock(keyData));
                        base.Send(skmb);

                        Session.Handshake = HandshakeType.ClientSymmetricKey;
                        NetDebuger.PrintDebugMessage(Session, "SEND ClientSymmetricKey");
                    }
                    break;

                case HandshakeType.ServerGetSymmetricKey:
                    {
                        NetDebuger.PrintDebugMessage(Session, "RECV ServerGetSymmetrickey");

                        Session.CheckPrePhase(HandshakeType.ClientSymmetricKey);
                        //Get crypt iv data
                        byte[] ivData = Singleton<RSACryptServiceBase>.Instance.Encrypt(
                            Session.SymmetricCryptService.SA.IV, Session.RemotePublicKey);

                        //Send Client Symmetric IV
                        MessageBlock ivmb = new MessageBlock(MessageBlockType.Handshake,
                            (int)HandshakeType.ClientSymmetricIV,
                            new DataBlock(ivData));
                        base.Send(ivmb);
                        Session.Handshake = HandshakeType.ClientSymmetricIV;
                        NetDebuger.PrintDebugMessage(Session, "SEND ClientSymmetricIV");
                    }
                    break;

                case HandshakeType.ServerGetSymmetricIV:
                    {
                        NetDebuger.PrintDebugMessage(Session, "RECV ServerGetSymmetricIV");

                        Session.CheckPrePhase(HandshakeType.ClientSymmetricIV);
                        MessageBlock mb2 = new MessageBlock(MessageBlockType.Handshake,
                            (int)HandshakeType.ClientFinished);
                        base.Send(mb2);
                        Session.Handshake = HandshakeType.ClientFinished;
                        NetDebuger.PrintDebugMessage(Session, "SEND ClientFinished");
                        break;
                    }

                case HandshakeType.ServerFinished:
                    {
                        Session.CheckPrePhase(HandshakeType.ClientFinished);
                        Session.Handshake = HandshakeType.OK;
                        NetDebuger.PrintDebugMessage(Session, "RECV ServerFinished. OK!");
                        isConnected = true;
                        StartHeartBeat();

                        //为了继续接收新数据，需要立刻返回，
                        //所以需要从新进程中调用OnBuildDataConnection();
                        buildConnThread = new Thread(OnBuildDataConnection);
                        buildConnThread.Start();
                    }
                    break;
            }
        }

        Thread buildConnThread;

        /// <summary>
        /// Called when [received message block].
        /// </summary>
        /// <param name="mb">The mb.</param>
        protected override void OnReceivedMessageBlock(MessageBlock mb)
        {
            switch(mb.Type)
            {
                case MessageBlockType.Handshake:
                    HandleHandshakeType((HandshakeType)mb.Parameter,mb);
                    break;
                case MessageBlockType.AppData:
                    if( !Session.IsBuildSecureConnection )
                    {
                        throw new NotBuildSecureConnectionException();
                    }

                    byte[] recvData =Session.SymmetricCryptService.Decrypt(mb.Body.ArraySegment);
                    OnReceivedData( new DataBlock(recvData));
                    break;
                case MessageBlockType.HeartBeat:
                    Debug.Assert(false);
                    break;
            }
        }

        /// <summary>
        /// Sends the specified mb.
        /// </summary>
        /// <param name="mb">The mb.</param>
        public override void Send(MessageBlock mb)
        {
            if (!Session.IsBuildSecureConnection)
            {
                throw new NotBuildSecureConnectionException();
            }

            if (mb.Body != null && mb.BodyLength!=0) //对数据部分进行加密后再传输
            {
                byte[] cryptData = Session.SymmetricCryptService.Encrypt(mb.Body.ArraySegment);
                mb = new MessageBlock(mb.Type, mb.Parameter, new DataBlock(cryptData));
            }

            base.Send(mb);
        }
    }
}
