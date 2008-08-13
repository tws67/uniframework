using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.ConnectionManagement
{
	/// <summary>
	/// This is the delegate used by the ConnectionManager to define the 
	/// event it uses to broadcast its state changes
	/// </summary>
	/// <param name="sender">Sender of the event</param>/>
	/// <param name="e">Event arguments</param>/>
	public delegate void ConnectionStateChangedEventHandler(object sender, 
		ConnectionStateChangedEventArgs e);

	/// <summary>
	/// EventArgs for ConnectionState changes. This class represents the connection states of the physical connection, 
	/// rather that the application.
	/// </summary>
	public class ConnectionStateChangedEventArgs : EventArgs
	{
		private ConnectionState newState;
		private ConnectionState oldState;

		/// <summary>
		/// Defines the data that is sent along with the event.
		/// </summary>
		/// <param name="oldState">State connection is transitioning from</param>
		/// <param name="newState">State connection is transitioning to</param>
		public ConnectionStateChangedEventArgs(ConnectionState oldState, ConnectionState newState)
		{
			this.newState = newState;
			this.oldState = oldState;
		}

		/// <summary>
		/// Returns the current physical state of the connection
		/// </summary>
		/// <value>The current connection state</value>
		public ConnectionState CurrentState { get { return newState; }}

		/// <summary>
		/// Returns the previous physical state of the connection
		/// </summary>
		/// <value>The original connection state</value>
		public ConnectionState OriginalState { get { return oldState; }}
	}

}
