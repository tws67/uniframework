using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Uniframework.Client.ConnectionManagement
{
	/// <summary>
	/// ConnectionDetector is responsible for determining if the application is online or
	/// offline at a "physical" level. There are pluggable strategies that can be configured
	/// to tailor how this determination is made through the IConnectionDetectionStrategy 
	/// interface.
	/// </summary>
	public class ConnectionDetector
	{
		private IConnectionDetectionStrategy connectionDetectionStrategy;
		private ConnectionManager connectionManager;
		private ConnectionState currentConnectionState = ConnectionState.Unknown;

		/// <summary>
		/// Constructor for ConnectionDetector
		/// </summary>
		/// <param name="connectionDetectionStrategy">The detection strategy chosen through the config file</param>
		/// <param name="connectionManager">The connection manager instance</param>
		public ConnectionDetector(IConnectionDetectionStrategy connectionDetectionStrategy, ConnectionManager connectionManager) 
		{
			this.connectionDetectionStrategy = connectionDetectionStrategy;
			this.connectionManager = connectionManager;
		}

		/// <summary>
		/// This method implements the finite state machine that deals with connection state changes.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DetectConnectionState()
		{
            bool online = CanGoOnline();
            if (online && ConnectedState != ConnectionState.Online)
            {
                TransitionToOnline();
            }
            else if (!online && ConnectedState != ConnectionState.Offline)
            {
                TransitionToOffline();
            }
		}

		/// <summary>
		/// Returns the connected state of the physical connection
		/// </summary>
		/// <value>Stores the enumerated value of the Connection State</value>
		public ConnectionState ConnectedState
		{
			get {return currentConnectionState;}
		}

		/// <summary>
		/// This method determines whether or not it is possible for to be placed online. It is possible to do so 
		/// if and only if the physical connection is in place. This method is useful if the application has been manually
		/// taken offline and wants to go back online.
		/// </summary>
		/// <returns>True if and only if the physical connection is currently connected. False otherwise</returns>
		public bool CanGoOnline()
		{
			return connectionDetectionStrategy.IsConnected();
		}

		/// <summary>
		/// This method forces the connection state to offline. It will remain in this state until ForceOnline is called
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ForceOffline()
		{
			if(ConnectedState != ConnectionState.Offline) 
			{
				TransitionToOffline();
			}
		}

		/// <summary>
		/// This method forces the connection state to online. It is only useful if we have been forced offline and there is a 
		/// physical connection present
		/// </summary>
		/// <exception>Throws ConnectionUnavailableException if CanGoOnline() would return false</exception>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ForceOnline()
		{
			if(ConnectedState != ConnectionState.Online)
			{
				TransitionToOnline();
			}
		}

		/// <summary>
		///  Implements the state change to Online
		/// </summary>
		private void TransitionToOnline()
		{
			ChangeToState(ConnectedState, ConnectionState.Online);
		}

		/// <summary>
		/// Encapsulates the setting of the currentConnectionState
		/// </summary>
		/// <param name="newState"></param>
		private void SetConnectedState(ConnectionState newState)
		{
			currentConnectionState = newState;
		}

		/// <summary>
		/// Implements the state transition to Offline. It is a method and not a property because the property already
		/// exists and is public. This is purely private functionality
		/// </summary>
		private void TransitionToOffline()
		{
			ChangeToState(ConnectedState, ConnectionState.Offline);
		}

		/// <summary>
		/// Implements the mechanics of changing states
		/// </summary>
		/// <param name="oldState">State we are coming from</param>
		/// <param name="newState">State we are going to</param>
		private void ChangeToState(ConnectionState oldState, ConnectionState newState)
		{
			SetConnectedState(newState);
			BroadcastStateChangedEvent(oldState, newState);
		}
		
		/// <summary>
		/// Causes the ConnectionManagerStateChangedEvent to be broadcast
		/// </summary>
		/// <param name="oldState">Previous state of the connection</param>
		/// <param name="newState">New state of the connection</param>
		private void BroadcastStateChangedEvent(ConnectionState oldState, ConnectionState newState)
		{
			ConnectionStateChangedEventArgs connectionStateChangedEventArgs = 
				new ConnectionStateChangedEventArgs(oldState, newState);
			this.connectionManager.RaiseConnectionStateChangedEvent(connectionStateChangedEventArgs);
		}

		/// <summary>
		/// Determines if this is a transition to online
		/// </summary>
		/// <returns></returns>
		private bool IsTransitioningToOnline()
		{
			return CanGoOnline() && 
				   ConnectedState != ConnectionState.Online;
		}

		/// <summary>
		/// Determines if this is a transition to offline
		/// </summary>
		/// <returns></returns>
		private bool IsTransitioningToOffline()
		{
			return CanGoOnline() == false &&
				   ConnectedState != ConnectionState.Offline;
		}
	}	
}
