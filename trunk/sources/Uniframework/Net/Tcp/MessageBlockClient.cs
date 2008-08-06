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
using System.Net;

namespace Uniframework.Net
{
    /// <summary>
    /// 基于消息的客户端类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class MessageBlockClient<TSession> : TcpClientBase<TSession> 
        where TSession : MessageBlockSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlockClient&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or address.</param>
        /// <param name="listenPort">The listen port.</param>
        public MessageBlockClient(string hostNameOrAddress, int listenPort)
            : base(hostNameOrAddress, listenPort)
        {
            EnableCheckHeartBeat = true; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBlockClient&lt;TSession&gt;"/> class.
        /// </summary>
        public MessageBlockClient()
        {
            EnableCheckHeartBeat = true; //默认启动检查心跳功能
        }

        /// <summary>
        /// 回话被创建，接收数据之前被调用
        /// </summary>
        protected override void OnCreateSession()
        {
            base.OnCreateSession();
            Session.OnReceivedMessageBlock += new EventHandler<MessageBlockArgs>(SessionOnReceivedMessageBlock);
        }

        /// <summary>
        /// Sessions the on received message block.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void SessionOnReceivedMessageBlock(object sender, MessageBlockArgs e)
        {
            if(e.MessageBlock.Type ==  MessageBlockType.HeartBeat && EnableCheckHeartBeat)
            {
                Session.TimeCounter.Reset(); //Refresh the heart Beat timer        
                NetDebuger.PrintDebugMessage(Session, "Recv server heart Beat");    
            }
            else
            {
                OnReceivedMessageBlock(e.MessageBlock);
            }
        }

        /// <summary>
        /// Called when [received message block].
        /// </summary>
        /// <param name="mb">The mb.</param>
        protected virtual void OnReceivedMessageBlock(MessageBlock mb)
        {
            OnReceivedData(mb.Body);
        }

        /// <summary>
        /// Sends the specified mb.
        /// </summary>
        /// <param name="mb">The mb.</param>
        public virtual void Send(MessageBlock mb)
        {
            base.Send(mb.ToDataBlock());
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public override void Send(byte[] data)
        {
            Send(new MessageBlock(data));
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public override void Send(byte[] data, int startIndex, int length)
        {
            Send(new MessageBlock(data, startIndex, length));
        }

        /// <summary>
        /// 发送数据块
        /// </summary>
        /// <param name="dataBlock"></param>
        public override void Send(DataBlock dataBlock)
        {
            Send(new MessageBlock(dataBlock));
        }

        /// <summary>
        /// 检查心跳的回调函数
        /// </summary>
        /// <param name="o">参数（未使用）</param>
        protected override void CheckHeartBeatCallBack(object o)
        {
            //If client is on line, go on send heart Beat singal
            if (IsConnected)
            {
                base.CheckHeartBeatCallBack(o);
            }

            if (IsConnected)//如果没有掉线，继续发送心跳信号
            {
                MessageBlock heartBeatMB = new MessageBlock(MessageBlockType.HeartBeat);
                Send(heartBeatMB);
                NetDebuger.PrintDebugMessage(Session, "Send Heart Beat");
            }
        }
    }
}
