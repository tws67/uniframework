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
    /// 文本命令服务器类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class CommandServer<TSession> : TcpServerBase<TSession>
        where TSession : CommandSession, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer&lt;TSession&gt;"/> class.
        /// </summary>
        public CommandServer()
        {
            this.Encoding = new UnicodeEncoding();
            NewLines = new string[] { "\r\n" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public CommandServer(int port)
            : base(port)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer&lt;TSession&gt;"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="capacity">The capacity.</param>
        public CommandServer(int port, int capacity)
            : base(port, capacity)
        {
        }

        private string[] newLines;

        /// <summary>
        /// Gets or sets the new lines.
        /// </summary>
        /// <value>The new lines.</value>
        public string[] NewLines
        {
            get { return newLines; }
            set { newLines = value; }
        }

        private Encoding encoding;

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
        /// Called when [create session].
        /// </summary>
        /// <param name="newSession">The new session.</param>
        /// <returns></returns>
        protected override bool OnCreateSession(TSession newSession)
        {
            newSession.Encoding = Encoding;
            newSession.NewLines = NewLines;
            newSession.ReceivedCommand += new EventHandler<TEventArgs<string>>(ReceivedCommand);
            return true;
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
        /// Called when [received command].
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="command">The command.</param>
        protected virtual void OnReceivedCommand(TSession session, string command)
        {
            NetDebuger.PrintDebugMessage(session, command);
        }

        /// <summary>
        /// Sends the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="cmd">The CMD.</param>
        public void Send(TSession session, string cmd)
        {
            Send(session, Encoding.GetBytes(cmd));
        }
    }
}
