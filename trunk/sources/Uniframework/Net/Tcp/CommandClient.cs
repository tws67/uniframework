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

namespace Uniframework.Net
{
    /// <summary>
    /// 文本命令客户端类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class CommandClient<TSession> : TcpClientBase<TSession> where TSession : CommandSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandClient&lt;TSession&gt;"/> class.
        /// </summary>
        public CommandClient()
        {
            Encoding = new UnicodeEncoding();
            NewLines = new string[] { "\r\n" };
        }

        private string[] newLines;
        private Encoding encoding;

        /// <summary>
        /// Gets or sets the new lines.
        /// </summary>
        /// <value>The new lines.</value>
        public string[] NewLines
        {
            get { return newLines; }
            set { newLines = value; }
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// 回话被创建，接收数据之前被调用
        /// </summary>
        protected override void OnCreateSession()
        {
            Session.Encoding = Encoding;
            Session.NewLines = this.NewLines;

            Session.ReceivedCommand += new EventHandler<TEventArgs<string>>(ReceivedCommand);
        }

        /// <summary>
        /// Receiveds the command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Uniframework.TEventArgs&lt;System.String&gt;"/> instance containing the event data.</param>
        private void ReceivedCommand(object sender, TEventArgs<string> e)
        {
            OnReceivedCommand((TSession)sender, e.Param);
        }

        /// <summary>
        /// 收到命令
        /// </summary>
        /// <param name="session"></param>
        /// <param name="command"></param>
        protected virtual void OnReceivedCommand(TSession session, string command)
        {
        }

        /// <summary>
        /// Sends the specified CMD.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        public void Send(string cmd)
        {
            Send(Encoding.GetBytes(cmd));
        }
    }
}
